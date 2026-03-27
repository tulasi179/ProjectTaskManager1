using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Projecttaskmanager.Models;

public class TaskDependency
{
     [Required]
    public int TaskId { get; set; }
    public int DependentTaskId { get; set; }

    // Navigation properties
    [JsonIgnore]
    public ProjectTasks? Task { get; set; } 
    [JsonIgnore]
    public ProjectTasks? DependentTask { get; set; } 
}