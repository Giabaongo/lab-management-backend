using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    // COMMENTED OUT - Test/Health check endpoint not needed in production
    /*
    /// <summary>
    /// Health check endpoint
    /// </summary>
    [Route("api/health")]
    [ApiController]
    public class TestConnectController : ControllerBase
    {
        /// <summary>
        /// Check API health status
        /// </summary>
        /// <returns>API status message</returns>
        [HttpGet]
        public IActionResult GetHealthStatus()
        {
            return Ok(new 
            { 
                status = "healthy",
                message = "API is running",
                timestamp = DateTime.UtcNow
            });
        }
    }
    */
}
