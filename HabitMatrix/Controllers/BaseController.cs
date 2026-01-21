using Microsoft.AspNetCore.Mvc;

namespace HabitMatrix.Controllers
{
    /// <summary>
    /// Base controller with helper methods for authentication and authorization.
    /// All other controllers can inherit from this.
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Check if the user is logged in by verifying the presence of a UserId claim.
        /// </summary>
        protected bool IsLoggedIn()
        {
            return User?.Claims.FirstOrDefault(c => c.Type == "UserId") != null;
        }

        /// <summary>
        /// Get current user's role from claims.
        /// </summary>
        protected string? GetUserRole()
        {
            return User?.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
        }


        /// <summary>
        /// Redirect to login if not authenticated.
        /// </summary>
        protected IActionResult EnsureAuthenticated()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            return null!;
        }

        /// <summary>
        /// Redirect to home if not admin.
        /// </summary>
        protected IActionResult EnsureAdmin()
        {
            if (GetUserRole() != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return null!;
        }
    }
}