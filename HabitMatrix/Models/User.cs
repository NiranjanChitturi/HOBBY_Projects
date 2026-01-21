using System;

namespace HabitMatrix.Models
{
    /// <summary>
    /// Represents an application user (Admin or User).
    /// Mirrors the SQL Users table, including soft delete and audit fields.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Primary key (UNIQUEIDENTIFIER in SQL).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Unique login name. Case-insensitive comparisons are recommended in code.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Hashed password (never store plain text). 
        /// Use a secure hashing algorithm (e.g., PBKDF2, BCrypt, or SHA-256 with salt).
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Contact email for notifications or password recovery.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Role-based access: "Admin" or "User".
        /// </summary>
        public string Role { get; set; } = "User";

        /// <summary>
        /// When the user was created (UTC).
        /// </summary>
        public DateTime? JoinedAt { get; set; }

        /// <summary>
        /// Last modification timestamp (UTC).
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Who modified this record (FK to Users.Id).
        /// </summary>
        public Guid? ModifiedBy { get; set; }

        /// <summary>
        /// Soft delete flag (true = hidden, not physically deleted).
        /// Always filter IsDeleted = false in queries.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Stored Users Theme Preference
        /// </summary>
        public string ThemePreference { get; set; } = "light";
    }
}