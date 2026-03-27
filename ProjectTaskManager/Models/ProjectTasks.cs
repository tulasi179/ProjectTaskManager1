using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Projecttaskmanager.Models;


public class ProjectTasks
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public int AssigneeId { get; set; }

    // Navigation properties
    [JsonIgnore]
    public Project? Project { get; set; } 
    [JsonIgnore]
    public Users? Assignee { get; set; }
    [JsonIgnore]
    public ICollection<TaskDependency> Dependencies { get; set; } = new List<TaskDependency>();
    [JsonIgnore]
    public ICollection<TaskDependency> Dependents { get; set; } = new List<TaskDependency>();

    
}

