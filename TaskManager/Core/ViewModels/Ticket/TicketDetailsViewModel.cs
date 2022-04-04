using TaskManager.Areas.Identity.Data;
using TaskManager.Models;

namespace TaskManager.Core.ViewModels.Ticket;

public class TicketDetailsViewModel
{
    public TaskManager.Models.Ticket Ticket { get; set; }

    public List<TicketAssignment> AssignedTo { get; set; }

    public Models.Project Project { get; set; }

    public ApplicationUser SubmittedBy { get; set; }

}
