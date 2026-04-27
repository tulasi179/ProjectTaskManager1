namespace Projecttaskmanager.Models;
public class OtpCode
{
    public int Id{ get; set; }
    public required string Email { get; set ;}
    public required string Code {get; set;}
    public required string Purpose {get ; set;}
    public DateTime ExpiresAt{ get; set;}
    public bool IsUsed {get; set;}= false;
    public DateTime CreatedAt {get; set;}= DateTime.UtcNow;

}