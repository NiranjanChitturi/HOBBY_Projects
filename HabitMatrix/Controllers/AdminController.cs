using HabitMatrix.Data;
using HabitMatrix.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HabitMatrix.Controllers
{
    /// <summary>
    /// Admin-only controller for managing users.
    /// Requires authentication and admin role.
    /// </summary>
    public class AdminController : BaseController
    {
        private readonly UserRepository _userRepo;

        public AdminController(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // ---------------------------
        // LIST USERS
        // ---------------------------
        public async Task<IActionResult> Users()
        {
            // Ensure logged in
            var redirectAuth = EnsureAuthenticated();
            if (redirectAuth != null) return redirectAuth;

            // Ensure admin
            var redirectRole = EnsureAdmin();
            if (redirectRole != null) return redirectRole;

            var users = await _userRepo.GetAllAsync();
            return View(users);
        }

        // ---------------------------
        // CHANGE ROLE (Admin ↔ User)
        // ---------------------------
        [HttpPost]
        public async Task<IActionResult> ChangeRole(Guid id, string role)
        {
            var redirectAuth = EnsureAuthenticated();
            if (redirectAuth != null) return redirectAuth;

            var redirectRole = EnsureAdmin();
            if (redirectRole != null) return redirectRole;

            var user = await _userRepo.GetByIdAsync(id);
            if (user != null)
            {
                user.Role = role;

                // ✅ Get adminId from claims instead of session
                var adminIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(adminIdClaim))
                    return RedirectToAction("Login", "Account");

                var adminId = Guid.Parse(adminIdClaim);

                await _userRepo.UpdateAsync(user, adminId);
            }

            return RedirectToAction("Users");
        }

        // ---------------------------
        // SOFT DELETE USER
        // ---------------------------
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var redirectAuth = EnsureAuthenticated();
            if (redirectAuth != null) return redirectAuth;

            var redirectRole = EnsureAdmin();
            if (redirectRole != null) return redirectRole;

            // ✅ Get adminId from claims instead of session
            var adminIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(adminIdClaim))
                return RedirectToAction("Login", "Account");

            var adminId = Guid.Parse(adminIdClaim);

            await _userRepo.SoftDeleteAsync(id, adminId);

            return RedirectToAction("Users");
        }
    }
}