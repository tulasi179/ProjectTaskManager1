using Projecttaskmanager.Migrations;

namespace Projecttaskmanager.Services;

public interface IEmailService
{
    
    Task SendOtpEmailAsync(string toEmail , string opt, string purpose);
}