using Flow_Api.Data.Contexts;
using Flow_Api.Dtos.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flow_Api.Controllers.Public
{
    [AllowAnonymous]
    public class HealthController : BaseApiController
    {
        private readonly MasterDbContext _context;

        public HealthController(MasterDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> CheckHealth()
        {
            try
            {
                // Check database connectivity
                await _context.Database.CanConnectAsync();

                var healthStatus = new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Database = "Connected",
                    Version = "1.0.0"
                };

                return Ok(ApiResponse<object>.SuccessResponse(healthStatus, "System is healthy"));
            }
            catch (Exception ex)
            {
                var healthStatus = new
                {
                    Status = "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Database = "Disconnected",
                    Error = ex.Message
                };

                return StatusCode(503, ApiResponse<object>.ErrorResponse(
                    "System is unhealthy",
                    new List<string> { ex.Message }
                ));
            }
        }
    }
}
