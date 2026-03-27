using System.ComponentModel.DataAnnotations;

namespace Projecttaskmanager.DTOs;

public class TaskRequestDto
{
    [Required]
    public int ProjectId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int AssigneeId { get; set; }
}