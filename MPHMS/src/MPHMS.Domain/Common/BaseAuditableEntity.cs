using System;

namespace MPHMS.Domain.Common
{
    /// <summary>
    /// BaseAuditableEntity is the ROOT base class for all domain entities
    /// that require auditing and soft delete support.
    ///
    /// This class maps directly to the standardized audit columns
    /// defined in the MPHMS database schema (v1.3).
    ///
    /// Why this exists:
    /// ----------------
    /// 1) Avoid repeating audit properties in every entity
    /// 2) Enforce consistency across the domain
    /// 3) Enable centralized EF Core audit automation
    /// 4) Support compliance-level traceability
    ///
    /// IMPORTANT:
    /// ----------
    /// This class is ONLY for BUSINESS entities.
    /// It is NOT used for ASP.NET Identity tables.
    ///
    /// Identity auditing is handled separately by the framework.
    /// </summary>
    public abstract class BaseAuditableEntity
    {
           public Guid Id { get; set; }

        /// <summary>
        /// Timestamp when the record was created.
        ///
        /// Stored in UTC to avoid timezone inconsistencies.
        ///
        /// Automatically populated by EF Core SaveChanges interceptor.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// UserId of the user who created this record.
        ///
        /// This links to UserProfiles.UserId (NOT Identity table).
        ///
        /// Example:
        /// --------
        /// Habit created by logged-in user
        /// CreatedBy = CurrentUserProfileId
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// Timestamp of the most recent modification.
        ///
        /// NULL for newly created records.
        ///
        /// Automatically updated during SaveChanges().
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// UserId of the user who last modified this record.
        ///
        /// Useful for:
        /// - Admin tracking
        /// - Debugging
        /// - Behavioral analytics
        /// </summary>
        public Guid? ModifiedBy { get; set; }

        /// <summary>
        /// Soft delete flag.
        ///
        /// TRUE  = logically deleted
        /// FALSE = active record
        ///
        /// IMPORTANT:
        /// ----------
        /// Records are NEVER physically deleted in MPHMS.
        /// This protects analytics integrity and audit history.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Timestamp when the record was soft deleted.
        ///
        /// NULL if record is active.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// UserId who performed the deletion action.
        ///
        /// Used mainly for:
        /// - Admin actions
        /// - Security auditing
        /// </summary>
        public Guid? DeletedBy { get; set; }
    }
}
