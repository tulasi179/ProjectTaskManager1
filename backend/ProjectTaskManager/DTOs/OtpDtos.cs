// DTOs/OtpDtos.cs
namespace Projecttaskmanager.DTOs;
public class SendOtpRequest
{
    public required string Email { get; set; }
    public  required string Purpose { get; set; }
}

public class VerifyOtpRequest
{
    public  required string Email { get; set; }
    public  required string Code { get; set; }
    public  required string Purpose { get; set; }
}