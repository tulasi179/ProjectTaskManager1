namespace Projecttaskmanager.Models;

public class OtpCode
{
    public int Id{ get; set; }
    public string Email { get; set ;}
    public string Code {get; set;}
    public string Purpose {get ; set;}
    public DateTime ExpiresAt{ get; set;}
    public bool IsUsed {get; set;}= false;
    public DateTime CreatedAt {get; set;}= DateTime.UtcNow;

}