using System.ComponentModel.DataAnnotations;
using TaskManager.Areas.Identity.Data;
using TaskManager.Utilities;

namespace TaskManager.Models;

public class Project
{
    public Guid Id { get; set; }

    
    [Required, StringLength(50)]
    public string Name { get; set; } = "";

    
    [Required, StringLength(1000)]
    public string Description { get; set; } = "";

    
    [StringLength(1000)]
    public string? DescriptionNoHtml { get; set; }

    public string? Tag { get; set; }

    //public bool Archived { get; set; } = false;

    [Display(Name= "Goal Date"), DataType(DataType.Date)]
    [ValidFutureDate(ErrorMessage = "Date must be in the future.")]
    public DateTime? GoalDate { get; set; }

    
    [Display(Name = "Created On"), DataType(DataType.Date)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    
    [Display(Name = "Updated At"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:g}")]
    public DateTime? UpdatedAt { get; set; }

    
    public string CreatedByUserId { get; set; } = String.Empty;

    public ApplicationUser? CreatedByUser { get; set; }


    public ICollection<ProjectAssignment>? Contributers { get; set; } = new List<ProjectAssignment>();

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public ICollection<PNote> Notes { get; set; } = new List<PNote>();

    public ICollection<ProjectFile> ProjectFiles { get; set; } = new List<ProjectFile>();

}
