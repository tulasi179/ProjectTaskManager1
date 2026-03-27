using System.ComponentModel.DataAnnotations;

namespace Projecttaskmanager.DTOs;

public class TaskStatusUpdateDto
{
    [Required]
    [RegularExpression("^(Pending|InProgress|Completed)$",
        ErrorMessage = "Status must be Pending, InProgress, or Completed.")]
    public string Status { get; set; } = null!;
}