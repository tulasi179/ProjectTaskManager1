using Microsoft.AspNetCore.Mvc;
using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;
using Projecttaskmanager.Services;
using Microsoft.AspNetCore.Authorization;

namespace Projecttaskmanager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
       
       [HttpPost("register")]
        public async Task<ActionResult<Users>> Register(UserResponce request)
        {
            var (user, error) = await authService.RegisterAsync(request);
            if (user is null)
                return BadRequest(error);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponce>> Login(UserResponce request)
        {
            var (result, error) = await authService.LoginAsync(request);
            if (result is null)
                return BadRequest(error);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponce>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are Admin!");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await authService.ResetPasswordAsync(dto);
            return Ok(new { message = "Password reset successfully." });
        }
    }
}