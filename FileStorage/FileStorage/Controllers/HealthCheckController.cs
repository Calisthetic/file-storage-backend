using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Controllers
{
    [Route("api/healthz")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpOptions]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> HealthCheck()
        {
            return Redirect("../healthz");
        }
    }
}
