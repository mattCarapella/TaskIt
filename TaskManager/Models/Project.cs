using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class Project
{
    public Guid Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = "";

    [Required, StringLength(1000)]
    public string Description { get; set; } = "";

    public string? Tag { get; set; }

    [Display(Name= "Goal Date"), DataType(DataType.Date)]
    public DateTime? GoalDate { get; set; }

    [Display(Name = "Created On"), DataType(DataType.Date)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Updated At"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:g}")]
    public DateTime? UpdatedAt { get; set; }



    public virtual ICollection<ProjectAssignment>? Contributers { get; set; } = new List<ProjectAssignment>();
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
