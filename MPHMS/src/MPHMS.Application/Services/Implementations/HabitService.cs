using MPHMS.Application.DTOs.Habits;
using MPHMS.Application.Repositories;
using MPHMS.Application.Services;
using MPHMS.Domain.Entities.Habits;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MPHMS.Application.Services.Implementations
{
    /// <summary>
    /// HabitService contains ALL business logic related to habits.
    ///
    /// Responsibilities:
    /// -----------------
    /// ✔ Create habits
    /// ✔ Update habits
    /// ✔ Soft delete habits
    /// ✔ Daily habit logging
    /// ✔ Skip reason tracking
    ///
    /// IMPORTANT:
    /// ----------
    /// - No EF Core usage
    /// - No HTTP logic
    /// - Uses repository abstractions ONLY
    /// </summary>
    public class HabitService : IHabitService
    {
        // WRITE repositories
        private readonly IGenericRepository<Habit> _habitRepository;
        private readonly IGenericRepository<HabitLog> _habitLogRepository;
        private readonly IGenericRepository<HabitSkipLog> _habitSkipRepository;

        // READ repositories
        private readonly IReadRepository<Habit> _habitReadRepository;
        private readonly IReadRepository<HabitLog> _habitLogReadRepository;

        // Unit of Work
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor Injection
        /// </summary>
        public HabitService(
            IGenericRepository<Habit> habitRepository,
            IReadRepository<Habit> habitReadRepository,
            IGenericRepository<HabitLog> habitLogRepository,
            IReadRepository<HabitLog> habitLogReadRepository,
            IGenericRepository<HabitSkipLog> habitSkipRepository,
            IUnitOfWork unitOfWork)
        {
            _habitRepository = habitRepository;
            _habitReadRepository = habitReadRepository;

            _habitLogRepository = habitLogRepository;
            _habitLogReadRepository = habitLogReadRepository;

            _habitSkipRepository = habitSkipRepository;
            _unitOfWork = unitOfWork;
        }

        // -------------------------------------------------------
        // CREATE HABIT
        // -------------------------------------------------------

        public async Task<Guid> CreateHabitAsync(CreateHabitRequest request)
        {
            var habit = new Habit
            {
                Name = request.Name,
                Difficulty = request.Difficulty,
                CategoryId = request.CategoryId,
                UserId = request.UserId,

                // ACTIVE by default
                Status = 1
            };

            await _habitRepository.AddAsync(habit);
            await _unitOfWork.SaveChangesAsync();

            return habit.Id;
        }

        // -------------------------------------------------------
        // UPDATE HABIT
        // -------------------------------------------------------

        public async Task UpdateHabitAsync(Guid habitId, UpdateHabitRequest request)
        {
            var habit = await _habitReadRepository.GetByIdAsync(habitId);

            if (habit == null)
                throw new Exception("Habit not found");

            habit.Name = request.Name;
            habit.Difficulty = request.Difficulty;
            habit.CategoryId = request.CategoryId;

            // Status replaces IsActive
            habit.Status = request.Status;

            _habitRepository.Update(habit);
            await _unitOfWork.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // DELETE HABIT (SOFT DELETE)
        // -------------------------------------------------------

        public async Task DeleteHabitAsync(Guid habitId)
        {
            var habit = await _habitReadRepository.GetByIdAsync(habitId);

            if (habit == null)
                throw new Exception("Habit not found");

            _habitRepository.Remove(habit);
            await _unitOfWork.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // DAILY LOGGING
        // -------------------------------------------------------

        public async Task LogHabitAsync(LogHabitRequest request)
        {
            var log = new HabitLog
            {
                HabitId = request.HabitId,

                // Correct DateOnly conversion
                LogDate = DateOnly.FromDateTime(DateTime.UtcNow),

                // Status replaces IsCompleted
                Status = request.IsCompleted ? 1 : 0
            };

            await _habitLogRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // SKIP REASON
        // -------------------------------------------------------

        public async Task AddSkipReasonAsync(AddSkipReasonRequest request)
        {
            var skip = new HabitSkipLog
            {
                HabitLogId = request.HabitLogId,
                ReasonId = request.ReasonId,
                Comment = request.Comment
            };

            await _habitSkipRepository.AddAsync(skip);
            await _unitOfWork.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // QUERY
        // -------------------------------------------------------

        public async Task<List<HabitResponse>> GetUserHabitsAsync(Guid userId)
        {
            var habits = await _habitReadRepository.FindAsync(h => h.UserId == userId);

            var response = new List<HabitResponse>();

            foreach (var habit in habits)
            {
                response.Add(new HabitResponse
                {
                    HabitId = habit.Id,
                    Name = habit.Name,
                    Difficulty = habit.Difficulty,
                    Status = habit.Status,
                    CategoryId = habit.CategoryId
                });
            }

            return response;
        }
    }
}
