namespace ProjectTaskManager.Services;

public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string otp);
}