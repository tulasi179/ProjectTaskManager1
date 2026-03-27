namespace Projecttaskmanager.DTOs
{
    public class TaskResponseDto
    {
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public int AssigneeId { get; set; }
    }
}