namespace Projecttaskmanager.DTOs;

public class NotificationResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool ReadStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}