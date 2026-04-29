using System.Net;
using System.Net.Mail;

namespace Projecttaskmanager.Services;

public class EmailService(IConfiguration config) : IEmailService
{
    public async Task SendOtpEmailAsync(string email, string otp, string purpose)
    {
        var subject = purpose == "registration"
            ? "Verify your Email - OTP"
            : "Reset your Password - OTP";

        var body = purpose == "registration"
            ? $@"<h2>Welcome!</h2>
                 <p>Your OTP for registration is:</p>
                 <h1 style='letter-spacing:8px'>{otp}</h1>
                 <p>This OTP is valid for <strong>10 minutes</strong>.</p>
                 <p>If you did not register, ignore this email.</p>"
            : $@"<h2>Password Reset</h2>
                 <p>Your OTP to reset your password is:</p>
                 <h1 style='letter-spacing:8px'>{otp}</h1>
                 <p>This OTP is valid for <strong>10 minutes</strong>.</p>
                 <p>If you did not request this, ignore this email.</p>";

        try
        {
          using var client = new SmtpClient(config["Email:SmtpHost"]!, 587)
            {
                Credentials = new NetworkCredential(config["Email:Username"], config["Email:Password"]),
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

           // Console.WriteLine($"Connecting to {config["Email:SmtpHost"]}:{config["Email:SmtpPort"]}...");


            //email object to be sent.
            var mail = new MailMessage
            {
                From = new MailAddress(config["Email:From"]!, "ProjectTaskManager"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(email);
            await client.SendMailAsync(mail);
            Console.WriteLine("✅ Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ SMTP ERROR: {ex.Message}");
            throw;
        }
    }
}