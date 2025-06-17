using AuthFlowPro.Application.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthFlowPro.API.Controllers
{
    [Route("api/test")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        [Authorize(Policy = Permissions.User.View)]
        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            return Ok("Only users with Permissions.Users.View can access this.");
        }
    }
}
