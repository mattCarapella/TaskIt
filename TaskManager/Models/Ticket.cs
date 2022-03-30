using System.ComponentModel.DataAnnotations;
using TaskManager.Areas.Identity.Data;

namespace TaskManager.Models;


public enum TicketType
{
    BUG = 0,
    FEATURE = 1,
}

public enum Status
{
    TODO,
    INPROGRESS = 1,
    SUBMITTED = 2,
    COMPLETED = 3
}

public class Ticket
{
    public Guid TicketId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Tag { get; set; }

    public TicketType? TicketType { get; set; }

    public Status? Status { get; set; } = 0;

    public int Upvotes { get; set; } = 0;

    public int? Priority { get; set; }

    [DataType(DataType.Date)]
    public DateTime? GoalDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [DataType(DataType.Date)]
    public DateTime? UpdatedAt { get; set; }

    public Project? Project { get; set; }

    public ApplicationUser? SubmittedBy { get; set; }

    public ApplicationUser? AssignedBy { get; set; }

    public ICollection<TicketAssignment>? AssignedTo { get; set; }

    public Guid? ProjectId { get; set; }

}



