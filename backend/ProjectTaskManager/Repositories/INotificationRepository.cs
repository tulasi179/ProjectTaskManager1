using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<Notification?> GetByIdAsync(int id);
    Task<List<Notification>> GetByUserIdAsync(int userId);
    Task<List<NotificationResponseDto>> GetAllAsync();
    Task SaveChangesAsync();
} 