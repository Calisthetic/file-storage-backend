using Asp.Versioning;
using FileStorage.Feedback.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Feedback.Controllers
{

    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/healthz")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IUserService _userService;

        public HealthCheckController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpOptions]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> HealthCheck()
        {
            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }
            return Redirect("../healthz");
        }
    }
}
