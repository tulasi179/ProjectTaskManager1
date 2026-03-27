using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Projecttaskmanager.Models;
public class Notification
{
    
    public int Id { get; set; }
    public int UserId { get; set; }

    [Required]
    public string message { get; set; } = string.Empty;

    public bool ReadStatus { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [JsonIgnore]
    public Users User { get; set; } = null!;
}