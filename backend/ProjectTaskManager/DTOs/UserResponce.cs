namespace Projecttaskmanager.DTOs;

public class UserResponce
{
     public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password {get ; set;} = string.Empty;
    public string Email { get; set; } = null!;
    //public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "User"; 
     public bool IsActive { get; set; }
}