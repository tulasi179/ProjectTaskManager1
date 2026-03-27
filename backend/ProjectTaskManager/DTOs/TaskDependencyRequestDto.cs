using System.ComponentModel.DataAnnotations;

namespace Projecttaskmanager.DTOs;

public class TaskDependencyRequestDto
{
    [Required]
    public int TaskId { get; set; }

    [Required]
    public int DependentTaskId { get; set; }
}