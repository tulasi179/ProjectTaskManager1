using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.Data;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Services;

public class OtpService(AppDbContext context, IEmailService emailService) : IOtpService
{
    public async Task SendOtpAsync(string email, string purpose)
    {
        // Invalidate any existing unused OTPs for this email + purpose
            var existing = await context.OtpCodes
                .Where(o => o.Email == email && o.Purpose == purpose && !o.IsUsed)
                .ToListAsync();
            context.OtpCodes.RemoveRange(existing);
            //makes all the entities in the existing for deletion doesnt delete automatically after save changes it gets deleted



        // Generate new 6-digit OTP
             var code = new Random().Next(100000, 999999).ToString();

        context.OtpCodes.Add(new OtpCode
        {
            Email = email,
            Code = code,
            Purpose = purpose,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        });

        await context.SaveChangesAsync();
        await emailService.SendOtpEmailAsync(email, code, purpose);
    }

    public async Task<bool> VerifyOtpAsync(string email, string code, string purpose)
    {
        var otpRecord = await context.OtpCodes
            .FirstOrDefaultAsync(o =>
                o.Email == email &&
                o.Code == code &&
                o.Purpose == purpose &&
                !o.IsUsed &&
                o.ExpiresAt > DateTime.UtcNow);

        if (otpRecord is null)
            return false;

        // Mark as used so it can't be reused
        otpRecord.IsUsed = true;
         if (purpose == "registration")
        {
            var user = await context.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user is not null)
                user.IsActive = true;
        }
        await context.SaveChangesAsync();

        return true;
    }
}