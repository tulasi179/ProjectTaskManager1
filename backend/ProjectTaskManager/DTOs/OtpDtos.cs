// DTOs/OtpDtos.cs
namespace Projecttaskmanager.DTOs;
public class SendOtpRequest
{
    public string Email { get; set; }
    public string Purpose { get; set; }
}

public class VerifyOtpRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
    public string Purpose { get; set; }
}