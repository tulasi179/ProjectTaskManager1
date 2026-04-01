using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Projecttaskmanager.Models;


public class Project
{
   public int Id { get; set; }
    public string Name { get; set; } = null!;

    public string? Description { get; set; } 

    [Required]
    public int OwnerId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Required]
    public DateTime EndDate { get; set; }

    // Navigation properties
    [JsonIgnore]
    public Users? Owner { get; set; } 
    [JsonIgnore]

    public ICollection<ProjectTasks> Tasks { get; set; } = new List<ProjectTasks>();


}