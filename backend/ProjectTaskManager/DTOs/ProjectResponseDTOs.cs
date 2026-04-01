namespace Projecttaskmanager.DTOs;

public class ProjectResponseDTOs
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int OwnerId { get; set; }
     public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}