using Microsoft.AspNetCore.Identity;
using System;

namespace MPHMS.Infrastructure.Identity
{
    /// <summary>
    /// ApplicationUser represents the AUTHENTICATION identity of a user.
    /// 
    /// IMPORTANT:
    /// ----------
    /// This class is NOT your business profile.
    /// It is used only for:
    /// - Login
    /// - Password hashing
    /// - Token generation
    /// - Account security
    /// 
    /// Business data such as profession, habits, goals, etc.
    /// will live in the UserProfiles table (Domain model).
    /// 
    /// Architecture:
    /// -------------
    /// ASP.NET Identity Tables:
    ///   AspNetUsers -> mapped using this class
    ///   AspNetRoles
    ///   AspNetUserRoles
    /// 
    /// MPHMS Business Tables:
    ///   UserProfiles -> linked using UserProfileId
    /// 
    /// This separation keeps:
    /// - Security isolated
    /// - Domain clean
    /// - Architecture scalable
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Foreign key reference to MPHMS UserProfiles table.
        /// 
        /// Purpose:
        /// --------
        /// This connects Identity user (login account)
        /// with the business profile (habits, goals, analytics).
        /// 
        /// Flow:
        /// -----
        /// 1) User registers
        /// 2) Identity creates ApplicationUser record
        /// 3) MPHMS creates UserProfiles record
        /// 4) This field stores the mapping
        /// </summary>
        public Guid UserProfileId { get; set; }

        /// <summary>
        /// Audit field indicating when the identity account was created.
        /// 
        /// Stored in UTC to avoid timezone inconsistencies.
        /// Useful for:
        /// - Security auditing
        /// - Account lifecycle analytics
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indicates whether this login account is active.
        /// 
        /// Purpose:
        /// --------
        /// Allows logical account suspension without deleting records.
        /// 
        /// Example:
        /// --------
        /// Admin disables suspicious account
        /// UserProfiles data still remains intact
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
