using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Controllers
{
    [Route("api/healthz")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> HealthCheck()
        {
            return Redirect("../healthz");
        }
    }
}
