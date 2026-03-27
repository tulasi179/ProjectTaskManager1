namespace Projecttaskmanager.Models;

public class Users
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "User"; 
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? RefreshToken {get; set;}
    public DateTime? RefreshTokenExpiryTime {get; set; }

    // // Navigation properties
     public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    public ICollection<ProjectTasks> AssignedTasks { get; set; } = new List<ProjectTasks>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    
}