namespace MPHMS.Application.Common.Interfaces
{
    /// <summary>
    /// Provides information about the currently authenticated user.
    ///
    /// Purpose:
    /// --------
    /// This abstraction allows Application layer to:
    /// ✔ Access UserId
    /// ✔ Remain framework independent
    /// ✔ Avoid HttpContext dependency
    ///
    /// Implemented in:
    /// ----------------
    /// Infrastructure / API layer (ASP.NET specific)
    ///
    /// Used in:
    /// --------
    /// Services, Business Logic, Auditing
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Returns authenticated UserId.
        /// Returns NULL if request is unauthenticated.
        /// </summary>
        Guid? UserId { get; }
    }
}
