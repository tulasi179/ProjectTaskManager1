using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;
using Projecttaskmanager.Repositories;

namespace Projecttaskmanager.Services;

public class NotificationService(INotificationRepository repo, IUserRepository userRepo) : INotificationService
{
    public async Task CreateNotification(int userId, string Message)
    {
        var notification = new Notification
        {
            UserId = userId,
            message = Message,
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(notification);
        await repo.SaveChangesAsync();
    }

    public async Task<List<Notification>> GetUserNotifications(int userId)
    {
        var user = await userRepo.GetEntityByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with Id {userId} was not found.");

        return await repo.GetByUserIdAsync(userId);
    }

    public async Task<List<NotificationResponseDto>> GetAllUserNotification()
        => await repo.GetAllAsync();

    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await repo.GetByIdAsync(notificationId);
        if (notification is null)
            throw new KeyNotFoundException($"Notification with Id {notificationId} was not found.");

        if (notification.UserId != userId)
            throw new UnauthorizedAccessException("You can only mark your own notifications as read.");

        notification.ReadStatus = true;
        await repo.SaveChangesAsync();
    }
}