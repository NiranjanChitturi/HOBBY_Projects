using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MPHMS.Api.Controllers.Base;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MPHMS.Api.Controllers.Auth
{
    /// <summary>
    /// AuthController handles Authentication related operations.
    ///
    /// Responsibilities:
    /// -----------------
    /// ✔ User login
    /// ✔ JWT token generation
    /// ✔ Claims assignment
    ///
    /// Architecture:
    /// -------------
    /// API Layer → Application Layer → Infrastructure
    ///
    /// NOTE:
    /// -----
    /// For Phase 1:
    /// - This uses MOCK login logic
    /// - Real DB user validation will be added in Phase 16 (Identity Integration)
    /// </summary>
    [Route("api/auth")]
    public class AuthController : BaseApiController
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor Injection
        /// </summary>
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // -------------------------------------------------------
        // LOGIN ENDPOINT
        // -------------------------------------------------------

        /// <summary>
        /// Authenticates user and returns JWT token.
        ///
        /// Temporary Demo Credentials:
        /// ---------------------------
        /// Username: admin
        /// Password: admin123
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // ---------------------------------------------------
            // TEMP AUTH CHECK (Will be replaced by DB validation)
            // ---------------------------------------------------

            if (request.Username != "admin" || request.Password != "admin123")
            {
                return Unauthorized("Invalid credentials");
            }

            // ---------------------------------------------------
            // JWT TOKEN GENERATION
            // ---------------------------------------------------

            var token = GenerateJwtToken();

            return ApiOk(new
            {
                Token = token,
                TokenType = "Bearer"
            });
        }

        // -------------------------------------------------------
        // JWT TOKEN CREATION METHOD
        // -------------------------------------------------------

        private string GenerateJwtToken()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = Convert.ToDouble(jwtSettings["TokenExpiryMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // ---------------------------------------------------
            // Claims (User Identity Data inside Token)
            // ---------------------------------------------------

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // -------------------------------------------------------
    // LOGIN REQUEST DTO (API Local DTO)
    // -------------------------------------------------------

    /// <summary>
    /// Login request payload.
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
