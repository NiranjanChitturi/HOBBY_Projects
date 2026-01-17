using MPHMS.Application.DTOs.Goals;
using MPHMS.Application.Repositories;
using MPHMS.Application.Services;
using MPHMS.Domain.Entities.Goals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MPHMS.Application.Common.Interfaces;

namespace MPHMS.Application.Services.Implementations
{
    /// <summary>
    /// GoalService contains ALL business logic related to Goals.
    /// </summary>
    public class GoalService : IGoalService
    {
        // WRITE repositories
        private readonly IGenericRepository<Goal> _goalRepository;
        private readonly IGenericRepository<Milestone> _milestoneRepository;

        // READ repositories
        private readonly IReadRepository<Goal> _goalReadRepository;
        private readonly IReadRepository<Milestone> _milestoneReadRepository;

        // Unit of Work
        private readonly IUnitOfWork _unitOfWork;

        //Current user service
        private readonly ICurrentUserService _currentUserService;

        public GoalService(
            IGenericRepository<Goal> goalRepository,
            IReadRepository<Goal> goalReadRepository,
            IGenericRepository<Milestone> milestoneRepository,
            IReadRepository<Milestone> milestoneReadRepository,
            IUnitOfWork unitOfWork,
ICurrentUserService currentUserService)

        {
            _goalRepository = goalRepository;
            _goalReadRepository = goalReadRepository;

            _milestoneRepository = milestoneRepository;
            _milestoneReadRepository = milestoneReadRepository;

            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        // -------------------------------------------------------
        // CREATE GOAL
        // -------------------------------------------------------

        public async Task<Guid> CreateGoalAsync(CreateGoalRequest request)
        {
            var goal = new Goal
            {
                Name = request.Name,
                Description = request.Description,

                UserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User not authenticated"),
                CategoryId = request.CategoryId,

                StartDate = request.StartDate,
                TargetDate = request.TargetDate,

                Status = 1
            };

            await _goalRepository.AddAsync(goal);
            await _unitOfWork.SaveChangesAsync();

            return goal.Id;
        }

        // -------------------------------------------------------
        // UPDATE GOAL
        // -------------------------------------------------------

        public async Task UpdateGoalAsync(Guid goalId, UpdateGoalRequest request)
        {
            var goal = await _goalReadRepository.GetByIdAsync(goalId);

            if (goal == null)
                throw new Exception("Goal not found");

            goal.Name = request.Name;
            goal.Description = request.Description;
            goal.CategoryId = request.CategoryId;
            goal.TargetDate = request.TargetDate;
            goal.Status = request.Status;

            _goalRepository.Update(goal);
            await _unitOfWork.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // DELETE GOAL
        // -------------------------------------------------------

        public async Task DeleteGoalAsync(Guid goalId)
        {
            var goal = await _goalReadRepository.GetByIdAsync(goalId);

            if (goal == null)
                throw new Exception("Goal not found");

            _goalRepository.Remove(goal);
            await _unitOfWork.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // MILESTONE
        // -------------------------------------------------------

        public async Task AddMilestoneAsync(AddMilestoneRequest request)
        {
            var milestone = new Milestone
            {
                GoalId = request.GoalId,
                Title = request.Title,
                TargetValue = request.TargetValue,
                CurrentValue = 0
            };

            await _milestoneRepository.AddAsync(milestone);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMilestoneProgressAsync(UpdateMilestoneProgressRequest request)
        {
            var milestone = await _milestoneReadRepository.GetByIdAsync(request.MilestoneId);

            if (milestone == null)
                throw new Exception("Milestone not found");

            milestone.CurrentValue = request.CurrentValue;

            _milestoneRepository.Update(milestone);
            await _unitOfWork.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // QUERY
        // -------------------------------------------------------

        public async Task<List<GoalResponse>> GetUserGoalsAsync(Guid userId)
        {
            var goals = await _goalReadRepository.FindAsync(g => g.UserId == userId);

            var response = new List<GoalResponse>();

            foreach (var goal in goals)
            {
                response.Add(new GoalResponse
                {
                    GoalId = goal.Id,
                    Name = goal.Name,
                    Description = goal.Description,
                    Status = goal.Status,
                    TargetDate = goal.TargetDate
                });
            }

            return response;
        }
    }
}
