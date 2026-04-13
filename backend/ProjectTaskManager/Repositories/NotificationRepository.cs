using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.Data;
using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Repositories;

public class NotificationRepository(AppDbContext context) : INotificationRepository
{
    public async Task AddAsync(Notification notification)
    {
        context.notify.Add(notification);
    }

    public async Task<Notification?> GetByIdAsync(int id)
        => await context.notify.FirstOrDefaultAsync(n => n.Id == id);

    public async Task<List<Notification>> GetByUserIdAsync(int userId)
        => await context.notify
            .Where(n => n.UserId == userId)
            .ToListAsync();

    public async Task<List<NotificationResponseDto>> GetAllAsync()
        => await context.notify.Select(c => new NotificationResponseDto
        {
            Id = c.Id,
            UserId = c.UserId,
            Message = c.message
        }).ToListAsync();

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();
}