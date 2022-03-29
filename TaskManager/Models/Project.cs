using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class Project
{
    public Guid Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    public string? Tag { get; set; }

    [DataType(DataType.Date)]
    public DateTime? GoalDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [DataType(DataType.Date)]
    public DateTime? UpdatedAt { get; set; }


    public ICollection<ProjectAssignment>? Contributers { get; set; } = new List<ProjectAssignment>();
}
