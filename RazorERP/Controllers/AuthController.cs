using Microsoft.AspNetCore.Mvc;
using RazorERP.Models;
using RazorERP.Services;

namespace RazorERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            try
            {
                var user = await _userService.Authenticate(userDto.Username, userDto.Password);
                if (user == null)
                    return Unauthorized(new { message = "Invalid username or password." });

                var token = _userService.GenerateJwtToken(user);

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
