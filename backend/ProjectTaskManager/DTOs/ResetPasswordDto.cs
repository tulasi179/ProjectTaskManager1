// DTOs/ResetPasswordDto.cs
namespace Projecttaskmanager.DTOs;

public class ResetPasswordDto
{
    public string Email { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}