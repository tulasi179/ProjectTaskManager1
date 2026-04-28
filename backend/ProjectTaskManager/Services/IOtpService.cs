namespace Projecttaskmanager.Services;

public interface IOtpService
{
    Task SendOtpAsync(string email, string purpose);
    
    Task<bool> VerifyOtpAsync(string email, string code, string purpose);
}