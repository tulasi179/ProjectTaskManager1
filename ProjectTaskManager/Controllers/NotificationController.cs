using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Projecttaskmanager.DTOs;
using Projecttaskmanager.Services;

namespace Projecttaskmanager.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    [HttpGet("my")]
   public async Task<IActionResult> GetMyNotifications()
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);//*
        var notifications = await notificationService.GetUserNotifications(currentUserId);

        var response = notifications.Select(n => new NotificationResponseDto
        {
            Id = n.Id,
            UserId = n.UserId,
            Message = n.message,
            ReadStatus = n.ReadStatus,
            CreatedAt = n.CreatedAt
        });

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserNotifications(int userId)
    {
        var notifications = await notificationService.GetUserNotifications(userId);
        return Ok(notifications);
    }


    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<NotificationResponseDto>>> GetAllUserNotification()
      => Ok( await notificationService.GetAllUserNotification());

    [Authorize]
    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await notificationService.MarkAsReadAsync(id, currentUserId);
        return Ok("Notification marked as read.");
    }
}