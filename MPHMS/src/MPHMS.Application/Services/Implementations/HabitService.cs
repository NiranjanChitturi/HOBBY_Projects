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
    /// This layer:
    /// - Contains ZERO EF Core code
    /// - Contains ZERO HTTP logic
    /// - Uses repository abstractions ONLY
    ///
    /// This enforces Clean Architecture separation.
    /// </summary>
    public class HabitService : IHabitService
    {
        // ----------------------------
        // Repository Dependencies
        // ----------------------------

        // Write repositories
        private readonly IGenericRepository<Habit> _habitRepository;
        private readonly IGenericRepository<HabitLog> _habitLogRepository;
        private readonly IGenericRepository<HabitSkipLog> _habitSkipRepository;

        // Read repositories
        private readonly IReadRepository<Habit> _habitReadRepository;
        private readonly IReadRepository<HabitLog> _habitLogReadRepository;

        // Transaction manager
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
        // HABIT CREATION
        // -------------------------------------------------------

        /// <summary>
        /// Creates a new habit.
        /// </summary>
        public async Task<Guid> CreateHabitAsync(CreateHabitRequest request)
        {
            var habit = new Habit
            {
                Name = request.Name,

                // Difficulty lookup value
                Difficulty = request.Difficulty,

                // Default ACTIVE status (lookup value)
                Status = 1,

                CategoryId = request.CategoryId,
                UserId = request.UserId
            };

            await _habitRepository.AddAsync(habit);
            await _unitOfWork.SaveChangesAsync();

            return habit.Id;
        }

        // -------------------------------------------------------
        // HABIT UPDATE
        // -------------------------------------------------------

        /// <summary>
        /// Updates existing habit details.
        /// </summary>
        public async Task UpdateHabitAsync(Guid habitId, UpdateHabitRequest request)
        {
            var habit = await _habitReadRepository.GetByIdAsync(habitId);

            if (habit == null)
                throw new Exception("Habit not found");

            habit.Name = request.Name;
            habit.Difficulty = request.Difficulty;
            habit.CategoryId = request.CategoryId;

            // Optional status update
            habit.Status = request.Status;

            _habitRepository.Update(habit);
            await _unitOfWork.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // HABIT DELETE (SOFT DELETE)
        // -------------------------------------------------------

        /// <summary>
        /// Soft deletes a habit.
        /// </summary>
        public async Task DeleteHabitAsync(Guid habitId)
        {
            var habit = await _habitReadRepository.GetByIdAsync(habitId);

            if (habit == null)
                throw new Exception("Habit not found");

            _habitRepository.Remove(habit);
            await _unitOfWork.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // DAILY HABIT LOGGING
        // -------------------------------------------------------

        /// <summary>
        /// Logs a habit completion entry.
        /// </summary>
        public async Task LogHabitAsync(LogHabitRequest request)
        {
            var log = new HabitLog
            {
                HabitId = request.HabitId,

                // Domain uses DateOnly
                LogDate = DateOnly.FromDateTime(DateTime.UtcNow),

                // Status lookup:
                // 1 = Completed
                // 2 = Skipped
                Status = request.IsCompleted ? 1 : 2
            };

            await _habitLogRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // SKIP REASON LOGGING
        // -------------------------------------------------------

        /// <summary>
        /// Adds skip reason for a habit log.
        /// </summary>
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
        // QUERY OPERATIONS
        // -------------------------------------------------------

        /// <summary>
        /// Returns all active habits for a user.
        /// </summary>
        public async Task<List<HabitResponse>> GetUserHabitsAsync(Guid userId)
        {
            var habits = await _habitReadRepository
                .FindAsync(x => x.UserId == userId);

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
