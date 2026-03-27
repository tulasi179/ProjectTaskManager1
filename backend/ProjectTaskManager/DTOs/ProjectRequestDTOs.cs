using System.ComponentModel.DataAnnotations;

namespace Projecttaskmanager.DTOs;

public class ProjectRequestDTOs
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    public int OwnerId { get; set; }

    [Required]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Required]
    public DateTime EndDate { get; set; }
}