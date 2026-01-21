using HabitMatrix.Data;
using HabitMatrix.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;

namespace HabitMatrix.Controllers
{
    /// <summary>
    /// Handles user authentication and theme preference for HabitMatrix.
    /// Responsibilities:
    /// - Register new users (secure password hashing).
    /// - Login existing users (verify password).
    /// - Logout (sign out).
    /// - Save theme preference (light/dark).
    /// Uses UserRepository for persistence.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepo;

        public AccountController(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // ---------------------------
        // REGISTER
        // ---------------------------

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            var users = await _userRepo.GetAllAsync();
            if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                ViewBag.Error = "Username already taken.";
                return View();
            }
            if (users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                ViewBag.Error = "Email already registered.";
                return View();
            }

            string hashedPassword = HashPassword(password);

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = hashedPassword,
                Role = "User",
                JoinedAt = DateTime.UtcNow,
                IsDeleted = false,
                ThemePreference = "light" // default theme
            };

            await _userRepo.AddAsync(newUser);

            return RedirectToAction("Login");
        }

        // ---------------------------
        // LOGIN
        // ---------------------------

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var users = await _userRepo.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null || user.IsDeleted)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            if (!VerifyPassword(password, user.PasswordHash))
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            // Issue claims
            var claims = new List<Claim>
    {
        new Claim("UserId", user.Id.ToString()),
        new Claim("Username", user.Username),
        new Claim("Role", user.Role),
        new Claim("ThemePreference", user.ThemePreference ?? "light")
    };

            var identity = new ClaimsIdentity(claims, "Cookies"); // ✅ scheme
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal); // ✅ scheme

            return RedirectToAction("Index", "Home");
        }

        // ---------------------------
        // LOGOUT
        // ---------------------------

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies"); // ✅ scheme
            return RedirectToAction("Login");
        }

        // ---------------------------
        // HELPER METHODS
        // ---------------------------

        private string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32
            );

            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            string stored = parts[1];

            byte[] entered = KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32
            );

            return Convert.ToBase64String(entered) == stored;
        }

        // ---------------------------
        // SAVE THEME
        // ---------------------------

        [HttpPost]
        public async Task<IActionResult> SaveTheme([FromBody] ThemeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Theme))
                return BadRequest("Theme value is required.");

            var userId = User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userRepo.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
                return NotFound();

            user.ThemePreference = request.Theme.ToLowerInvariant();

            // ✅ Pass modifiedBy to UpdateAsync
            await _userRepo.UpdateAsync(user, Guid.Parse(userId));

            // Update claims immediately
            var identity = (ClaimsIdentity)User.Identity;
            var existingClaim = identity.FindFirst("ThemePreference");
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);
            identity.AddClaim(new Claim("ThemePreference", user.ThemePreference));

            return Ok(new { theme = user.ThemePreference });
        }
    }

    // ✅ Move ThemeRequest outside controller to avoid duplicate definitions
    public class ThemeRequest
    {
        public string Theme { get; set; } = "light";
    }
}