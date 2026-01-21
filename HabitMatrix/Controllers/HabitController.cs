using HabitMatrix.Data;
using HabitMatrix.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace HabitMatrix.Controllers
{
    /// <summary>
    /// Handles habit board (matrix grid) and logging daily progress.
    /// Requires authentication.
    /// </summary>
    public class HabitController : BaseController
    {
        private readonly HabitRepository _habitRepo;
        private readonly HabitLogRepository _logRepo;
        private readonly HabitSuggestionRepository _suggestionRepo;
        private readonly HabitCategoryRepository _categoryRepo;

        public HabitController(
            HabitRepository habitRepo,
            HabitLogRepository logRepo,
            HabitSuggestionRepository suggestionRepo,
            HabitCategoryRepository categoryRepo)
        {
            _habitRepo = habitRepo;
            _logRepo = logRepo;
            _suggestionRepo = suggestionRepo;
            _categoryRepo = categoryRepo;
        }

        // ---------------------------
        // HABIT BOARD
        // ---------------------------
        public async Task<IActionResult> Board()
        {
            var redirect = EnsureAuthenticated();
            if (redirect != null) return redirect;

            // ✅ Get userId from claims instead of session
            var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            var userId = Guid.Parse(userIdClaim);

            var habits = await _habitRepo.GetByUserAsync(userId);

            // Fetch logs for each habit
            var logsDict = new Dictionary<Guid, List<HabitLog>>();
            foreach (var habit in habits)
            {
                logsDict[habit.Id] = await _logRepo.GetByHabitAsync(habit.Id);
            }

            ViewBag.Habits = habits;
            ViewBag.LogsDict = logsDict;

            return View();
        }

        // ---------------------------
        // TOGGLE LOG STATUS
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLog(Guid habitId, DateTime logDate, bool status)
        {
            var redirect = EnsureAuthenticated();
            if (redirect != null) return redirect;

            // ✅ Get userId from claims
            var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            var userId = Guid.Parse(userIdClaim);

            var existing = await _logRepo.GetByHabitAndDateAsync(habitId, logDate);

            if (existing == null)
            {
                var log = new HabitLog
                {
                    Id = Guid.NewGuid(),
                    HabitId = habitId,
                    LogDate = DateOnly.FromDateTime(logDate),
                    Status = status,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                await _logRepo.CreateAsync(log);
            }
            else
            {
                existing.Status = status;
                var rows = await _logRepo.UpdateAsync(existing, userId);
                if (rows == 0)
                {
                    throw new InvalidOperationException($"No HabitLog updated for Id={existing.Id}");
                }
            }

            return Json(new { status });
        }

        // ---------------------------
        // HABIT SUGGESTIONS (VIEW)
        // ---------------------------
        public async Task<IActionResult> Suggestions(int categoryId)
        {
            var redirect = EnsureAuthenticated();
            if (redirect != null) return redirect;

            var suggestions = await _suggestionRepo.GetByCategoryAsync(categoryId);
            return View(suggestions);
        }

        // ---------------------------
        // CREATE HABIT
        // ---------------------------
        [HttpPost]
        public async Task<IActionResult> Create(string Name, int CategoryId, string Category)
        {
            var redirect = EnsureAuthenticated();
            if (redirect != null) return redirect;

            var userId = GetUserIdFromClaims();

            var habit = new Habit
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = Name,
                CategoryId = CategoryId,
                Category = Category,
                Color = "#00BFFF", // default color
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _habitRepo.CreateAsync(habit);
            return RedirectToAction("Board");
        }

        // ---------------------------
        // ADD HABIT (AJAX)
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHabit(string category, string habitName, string habitColor)
        {
            var userId = GetUserIdFromClaims();

            var habit = new Habit
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Category = category,
                Name = habitName,
                Color = habitColor,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _habitRepo.CreateAsync(habit);

            return Json(new { id = habit.Id, name = habit.Name, category = habit.Category, color = habit.Color });
        }

        // ---------------------------
        // GET CATEGORIES (Dropdown)
        // ---------------------------
        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return Json(categories.Select(c => new { c.Id, c.Name }));
        }

        // ---------------------------
        // GET SUGGESTIONS (Autocomplete)
        // ---------------------------
        [HttpGet]
        public async Task<IActionResult> Suggestions(int categoryId, string query = "")
        {
            var suggestions = await _suggestionRepo.GetByCategoryAsync(categoryId);

            if (!string.IsNullOrEmpty(query))
                suggestions = suggestions
                    .Where(s => s.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return Json(suggestions.Select(s => s.Title).Distinct().Take(10));
        }

        // ---------------------------
        // HELPER: Get UserId from claims
        // ---------------------------
        private Guid GetUserIdFromClaims()
        {
            var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new InvalidOperationException("User is not logged in or claim missing.");

            return Guid.Parse(userIdClaim);
        }
    }
}