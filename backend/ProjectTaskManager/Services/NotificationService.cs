using Projecttaskmanager.Data;
using Projecttaskmanager.Models;
using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.DTOs;
namespace Projecttaskmanager.Services;

public class NotificationService(AppDbContext context) : INotificationService
{
    
    public async Task CreateNotification(int userId, string Message)
    {
        var notification = new Notification
        {
            UserId = userId,
            message = Message,
            CreatedAt = DateTime.UtcNow  
        };

        context.notify.Add(notification);
        await context.SaveChangesAsync();
    }

    public async Task<List<Notification>>  GetUserNotifications(int userId)
    {
        var user = await context.User.FindAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with Id {userId} was not found.");
        return await context.notify
            .Where(n => n.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<NotificationResponseDto>> GetAllUserNotification()
         => await context.notify.Select(c => new NotificationResponseDto
     {
        Id  = c.Id,
         UserId = c.UserId,
         Message = c.message

     }).ToListAsync();

    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await context.notify
            .FirstOrDefaultAsync(n => n.Id == notificationId);

        if (notification is null)
            throw new KeyNotFoundException($"Notification with Id {notificationId} was not found.");

        // Users can only mark their own notifications as read
        if (notification.UserId != userId)
            throw new UnauthorizedAccessException("You can only mark your own notifications as read.");

        notification.ReadStatus = true;
        await context.SaveChangesAsync();
    }
}