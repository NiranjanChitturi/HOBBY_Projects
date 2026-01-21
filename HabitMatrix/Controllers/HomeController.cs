using HabitMatrix.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitMatrix.Controllers
{
    /// <summary>
    /// Dashboard controller. Shows user habits and logs after login.
    /// </summary>
    [Authorize]

    public class HomeController : BaseController
    {
        private readonly HabitRepository _habitRepo;
        private readonly HabitLogRepository _logRepo;

        public HomeController(HabitRepository habitRepo, HabitLogRepository logRepo)
        {
            _habitRepo = habitRepo;
            _logRepo = logRepo;
        }

        /// <summary>
        /// Dashboard page. Requires authentication.
        /// </summary>
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // ✅ Get current userId from claims
            var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            var userId = Guid.Parse(userIdClaim);

            // ✅ Fetch all habits for this user
            var habits = await _habitRepo.GetByUserAsync(userId);

            // ✅ Fetch logs (currently only for first habit — extend later to all habits)
            var logs = habits.Any()
                ? await _logRepo.GetByHabitAsync(habits.First().Id)
                : new List<Models.HabitLog>();

            // ✅ Recent logs (latest 5)
            var recentLogs = logs
                .OrderByDescending(l => l.LogDate)
                .Take(5)
                .ToList();

            // ✅ Trend data (last 7 days ✔ vs ✘)
            var trendData = logs
                .Where(l => l.LogDate >= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)))
                .GroupBy(l => l.LogDate)
                .Select(g => new
                {
                    Date = g.Key.ToString("dd-MM"),
                    Completed = g.Count(x => x.Status),
                    Missed = g.Count(x => !x.Status)
                })
                .ToList();

            // ✅ Category distribution
            var categoryCounts = habits
                .GroupBy(h => h.Category)
                .ToDictionary(g => g.Key, g => g.Count());

            // ✅ Weekly completion rate
            var last7DaysLogs = logs.Where(l => l.LogDate >= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7))).ToList();
            double weeklyRate = 0;
            if (last7DaysLogs.Any())
            {
                var completed = last7DaysLogs.Count(l => l.Status);
                weeklyRate = (double)completed / last7DaysLogs.Count * 100;
            }

            // ✅ Streak data
            var streakData = new Dictionary<string, int>();
            foreach (var habit in habits)
            {
                var habitLogs = await _logRepo.GetByHabitAsync(habit.Id);
                int streak = 0, maxStreak = 0;
                foreach (var log in habitLogs.OrderBy(l => l.LogDate))
                {
                    if (log.Status)
                    {
                        streak++;
                        maxStreak = Math.Max(maxStreak, streak);
                    }
                    else
                    {
                        streak = 0;
                    }
                }
                streakData[habit.Name] = maxStreak;
            }

            // ✅ Top 5 consistent habits
            var topHabits = habits
                .Select(h =>
                {
                    var habitLogs = logs.Where(l => l.HabitId == h.Id).ToList();
                    double rate = habitLogs.Any()
                        ? (double)habitLogs.Count(l => l.Status) / habitLogs.Count * 100
                        : 0;
                    return new { h.Name, Rate = rate };
                })
                .GroupBy(x => x.Name)
                .Select(g => new { Name = g.Key, Rate = g.Average(x => x.Rate) })
                .OrderByDescending(x => x.Rate)
                .Take(5)
                .ToDictionary(x => x.Name, x => x.Rate);

            // ✅ Pass data to view using claims
            ViewBag.Username = User?.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;
            ViewBag.Role = User?.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
            ViewBag.Habits = habits;
            ViewBag.Logs = logs;
            ViewBag.RecentLogs = recentLogs;
            ViewBag.TrendData = trendData;
            ViewBag.CategoryCounts = categoryCounts;
            ViewBag.WeeklyRate = weeklyRate;
            ViewBag.StreakData = streakData;
            ViewBag.TopHabits = topHabits;

            return View();
        }
    }
}