using System.ComponentModel.DataAnnotations;
using TaskManager.Areas.Identity.Data;
using TaskManager.Utilities;
using static TaskManager.Core.Enums.Enums;

namespace TaskManager.Models;

public class Ticket
{
    public Guid TicketId { get; set; }


    [Required, StringLength(50)]
    public string Title { get; set; } = "";


    [Required, StringLength(2000)]
    public string Description { get; set; } = "";


    [StringLength(2000)]
    public string? DescriptionNoHtml { get; set; }


    public string? Tag { get; set; }


    [Display(Name = "Ticket Type")]
    public TicketType? TicketType { get; set; }


    [Required(ErrorMessage = "Ticket status is required"), Display(Name = "Ticket Status")]
    public Status Status { get; set; } = 0;


    public Priority? Priority { get; set; }


    public int Upvotes { get; set; } = 0;


    [DataType(DataType.Date), Display(Name = "Goal Date")]
    [ValidFutureDate(ErrorMessage = "Date must be in the future.")]
    public DateTime? GoalDate { get; set; }


    [DataType(DataType.Date), Display(Name = "Created On")]
    [DisplayFormat(DataFormatString = "{0:g}")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;


    [DataType(DataType.Date), Display(Name = "Last Updated At")]
    [DisplayFormat(DataFormatString ="{0:g}")]
    public DateTime? UpdatedAt { get; set; }


    [DataType(DataType.Date), Display(Name = "Closed At")]
    [DisplayFormat(DataFormatString = "{0:g}")]
    public DateTime? ClosedAt { get; set; }


    [Display(Name = "Submitted by")]
    public ApplicationUser SubmittedBy { get; set; }
    public string SubmittedById { get; set;  }


    public ICollection<TicketAssignment> AssignedTo { get; set; } = new List<TicketAssignment>();

    public ICollection<TNote> TNotes { get; set; } = new List<TNote>();


    public Project? Project { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

}



