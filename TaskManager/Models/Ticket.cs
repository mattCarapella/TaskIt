using System.ComponentModel.DataAnnotations;
using TaskManager.Areas.Identity.Data;
using static TaskManager.Core.Enums.Enums;

namespace TaskManager.Models;


//public enum TicketType
//{
//    BUG = 0,
//    FEATURE = 1,
//}

//public enum Status
//{
//    TODO,
//    INPROGRESS = 1,
//    SUBMITTED = 2,
//    COMPLETED = 3
//}

public class Ticket
{
    public Guid TicketId { get; set; }

    [Required, StringLength(50)]
    public string? Title { get; set; }

    [Required, StringLength(2000)]
    public string? Description { get; set; }

    public string? Tag { get; set; }

    [Display(Name = "Ticket Type")]
    public TicketType? TicketType { get; set; }

    [Required(ErrorMessage = "Ticket status is required"), Display(Name = "Ticket Status")]
    public Status? Status { get; set; } = 0;

    public int Upvotes { get; set; } = 0;

    [Range(1,5)]
    public int? Priority { get; set; }

    [DataType(DataType.Date), Display(Name = "Goal Date")]
    public DateTime? GoalDate { get; set; }

    [DataType(DataType.Date), Display(Name = "Created On")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [DataType(DataType.Date), Display(Name = "Updated At")]
    [DisplayFormat(DataFormatString ="{0:g}")]
    public DateTime? UpdatedAt { get; set; }

    public Project? Project { get; set; }

    [Display(Name = "Submitted by")]
    public ApplicationUser? SubmittedBy { get; set; }

    [Display(Name = "Assigned by")]
    public ApplicationUser? AssignedBy { get; set; }

    public ICollection<TicketAssignment>? AssignedTo { get; set; }

    public Guid? ProjectId { get; set; }

}



