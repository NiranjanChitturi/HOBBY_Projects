using HabitMatrix.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HabitMatrix.Controllers
{
    public class HealthCheckController : Controller
    {
        private readonly DatabaseService _db;

        public HealthCheckController(DatabaseService db)
        {
            _db = db;
        }

        [HttpGet("/health")]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Simple query to validate DB connectivity
                var table = await _db.ExecuteQueryAsync("SELECT TOP 1 GETDATE() AS CurrentTime");

                var currentTime = table.Rows[0]["CurrentTime"];
                return Ok(new { status = "Healthy", dbTime = currentTime });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Unhealthy", error = ex.Message });
            }
        }
    }
}