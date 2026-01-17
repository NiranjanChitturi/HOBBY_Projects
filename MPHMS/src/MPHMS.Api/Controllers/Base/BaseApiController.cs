using Microsoft.AspNetCore.Mvc;

namespace MPHMS.Api.Controllers.Base
{
    /// <summary>
    /// BaseApiController is the ROOT controller class
    /// for all MPHMS API endpoints.
    ///
    /// Why this exists:
    /// ----------------
    /// ? Avoid duplicate attributes on every controller
    /// ? Centralize API behavior
    /// ? Enforce consistent routing
    /// ? Prepare for cross-cutting concerns (Auth, Versioning, Filters)
    ///
    /// Architecture Layer:
    /// -------------------
    /// API Layer (Presentation)
    ///
    /// IMPORTANT:
    /// ----------
    /// All controllers MUST inherit from this class.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Returns standardized OK response.
        ///
        /// Example:
        /// --------
        /// return ApiOk(data);
        /// </summary>
        protected IActionResult ApiOk(object? data = null)
        {
            return Ok(new
            {
                success = true,
                data = data
            });
        }

        /// <summary>
        /// Returns standardized Created response.
        ///
        /// Used for POST operations.
        /// </summary>
        protected IActionResult ApiCreated(object? data = null)
        {
            return Created(string.Empty, new
            {
                success = true,
                data = data
            });
        }

        /// <summary>
        /// Returns standardized BadRequest response.
        /// </summary>
        protected IActionResult ApiBadRequest(string message)
        {
            return BadRequest(new
            {
                success = false,
                error = message
            });
        }

        /// <summary>
        /// Returns standardized NotFound response.
        /// </summary>
        protected IActionResult ApiNotFound(string message)
        {
            return NotFound(new
            {
                success = false,
                error = message
            });
        }

        /// <summary>
        /// Returns standardized Server Error response.
        ///
        /// This will be used by global exception filters later.
        /// </summary>
        protected IActionResult ApiServerError(string message)
        {
            return StatusCode(500, new
            {
                success = false,
                error = message
            });
        }
    }
}
