using Projecttaskmanager.Services;
using Microsoft.AspNetCore.Mvc;
using Projecttaskmanager.DTOs;

namespace Projecttaskmanager.Models;

// Controllers/OtpController.cs
[ApiController]
[Route("api/[controller]")]

public class OtpController(IOtpService otpService) : ControllerBase
{
    // POST api/otp/send
    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Purpose))
            return BadRequest(new { message = "Email and purpose are required." });

        if (request.Purpose != "registration" && request.Purpose != "forgot-password")
            return BadRequest(new { message = "Invalid purpose." });

        await otpService.SendOtpAsync(request.Email, request.Purpose);
        return Ok(new { message = "OTP sent successfully." });
    }

    // POST api/otp/verify
    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Code) ||
            string.IsNullOrWhiteSpace(request.Purpose))
            return BadRequest(new { message = "Email, code and purpose are required." });

        var isValid = await otpService.VerifyOtpAsync(request.Email, request.Code, request.Purpose);

        if (!isValid)
            return BadRequest(new { message = "Invalid or expired OTP." });

        return Ok(new { message = "OTP verified successfully.", verified = true });
    }
}