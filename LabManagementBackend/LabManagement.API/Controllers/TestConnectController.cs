using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestConnectController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API is running");
        }
    }
}
