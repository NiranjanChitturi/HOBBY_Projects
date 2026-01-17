using MPHMS.Application.DTOs.Goals;
using MPHMS.Application.Repositories;
using MPHMS.Application.Services;
using MPHMS.Domain.Entities.Goals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MPHMS.Application.Services.Implementations
{
    /// <summary>
    /// GoalService contains ALL business logic related to Goals.
    ///
    /// Responsibilities:
    /// -----------------
    /// ✔ Create goals
    /// ✔ Update goals
    /// ✔ Soft delete goals
    /// ✔ Manage milestones
    /// ✔ Fetch user goals
    ///
    /// IMPORTANT:
    /// ----------
    /// - No EF Core usage
    /// - No HTTP logic
    /// - No infrastructure dependency
    ///
    /// Uses repository abstractions ONLY.
    /// </summary>
    public class GoalService : IGoalService
    {
        // ----------------------------
        // Repository Dependencies
        // ----------------------------

        private readonly IGenericRepository<Goal> _goalRepository;
        private readonly IReadRepository<Goal> _goalReadRepository;

        private readonly IGenericRepository<Milestone> _milestoneRepository;
        private readonly IReadRepository<Milestone> _milestoneReadRepository;

        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor Injection
        /// </summary>
        public GoalService(
            IGenericRepository<Goal> goalRepository,
            IReadRepository<Goal> goalReadRepository,
            IGenericRepository<Milestone> milestoneRepository,
            IReadRepository<Milestone> milestoneReadRepository,
            IUnitOfWork unitOfWork)
        {
            _goalRepository = goalRepository;
            _goalReadRepository = goalReadRepository;

            _milestoneRepository = milestoneRepository;
            _milestoneReadRepository = milestoneReadRepository;

            _unitOfWork = unitOfWork;
        }

        // -------------------------------------------------------
        // GOAL CREATION
        // -------------------------------------------------------

        /// <summary>
        /// Creates a new goal.
        /// </summary>
        public async Task<Guid> CreateGoalAsync(CreateGoalRequest request)
        {
            var goal = new Goal
            {
                Name = request.Name,
                Description = request.Description,

                UserId = request.UserId,
                CategoryId = request.CategoryId,

                StartDate = request.StartDate,
                TargetDate = request.TargetDate,

                Status = 1 // Default ACTIVE
            };

            await _goalRepository.AddAsync(goal);
            await _unitOfWork.SaveChangesAsync();

            return goal.Id;
        }

        // -------------------------------------------------------
        // GOAL UPDATE
        // -------------------------------------------------------

        /// <summary>
        /// Updates existing goal details.
        /// </summary>
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
        // GOAL DELETE (SOFT DELETE)
        // -------------------------------------------------------

        /// <summary>
        /// Soft deletes a goal.
        /// </summary>
        public async Task DeleteGoalAsync(Guid goalId)
        {
            var goal = await _goalReadRepository.GetByIdAsync(goalId);

            if (goal == null)
                throw new Exception("Goal not found");

            _goalRepository.Remove(goal);
            await _unitOfWork.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // MILESTONE MANAGEMENT
        // -------------------------------------------------------

        /// <summary>
        /// Adds milestone to a goal.
        /// </summary>
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

        /// <summary>
        /// Updates milestone progress.
        /// </summary>
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
        // QUERY OPERATIONS
        // -------------------------------------------------------

        /// <summary>
        /// Returns all goals for a user.
        /// </summary>
        public async Task<List<GoalResponse>> GetUserGoalsAsync(Guid userId)
        {
            var goals = await _goalReadRepository
                .FindAsync(g => g.UserId == userId);

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
