using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RazorERP.Models;
using RazorERP.Services;
using System.Security.Claims;

namespace RazorERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("company/{companyId}/users")]
        public async Task<IActionResult> GetUsersByCompanyId(int companyId)
        {
            try
            {
                var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (userRole == null)
                {
                    return Unauthorized(new { message = "User role not found." });
                }

                var users = await _userService.GetUsersByCompanyId(companyId, userRole);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            try
            {
                var result = await _userService.CreateUser(userDto, userDto.Password);
                if (result > 0)
                    return Ok(new { message = "User created successfully." });

                return BadRequest(new { message = "User creation failed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            try
            {
                var result = await _userService.UpdateUser(id, userDto, userDto.Password);
                if (result > 0)
                    return Ok(new { message = "User updated successfully." });

                return BadRequest(new { message = "User update failed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUser(id);
                if (result > 0)
                    return Ok(new { message = "User deleted successfully." });

                return BadRequest(new { message = "User deletion failed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
