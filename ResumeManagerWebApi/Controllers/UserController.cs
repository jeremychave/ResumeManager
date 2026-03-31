using Microsoft.AspNetCore.Mvc;
using ResumeManagerWebApi.Controllers.Dtos.User;
using ResumeManagerWebApi.Services.User;

namespace ResumeManagerWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncUser(SyncUserRequestDto request)
        {
            await _userService.SyncUser(request.Email);
            return Ok();
        }
    }
}
