using Microsoft.AspNetCore.Http;
using MPHMS.Application.Common.Interfaces;
using System;
using System.Security.Claims;

namespace MPHMS.Api.Services
{
    /// <summary>
    /// Provides the currently authenticated user's ID.
    ///
    /// Reads user identifier from HTTP Context claims.
    /// This keeps HTTP dependencies OUT of Application layer.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the current authenticated user's ID.
        ///
        /// Returns NULL if user is not authenticated.
        /// Controller or Service layer should enforce authorization.
        /// </summary>
        public Guid? UserId
        {
            get
            {
                var userIdClaim =
                    _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userIdClaim))
                    return null;

                return Guid.Parse(userIdClaim);
            }
        }
    }
}
