using TaskManager.Models;

namespace TaskManager.Core.ViewModels.Project;

public class ProjectDetailsViewModel
{
    public TaskManager.Models.Project Project { get; set; }

    public ICollection<ProjectAssignment> Contributers { get; set; }

    public ICollection<Models.Ticket> AllOpenTickets { get; set; }

    public PaginatedList<Models.Ticket> OpenTicketsPaginated { get; set; }

    public PaginatedList<Models.Ticket> ClosedTicketsPaginated { get; set; }

    public ICollection<PNote> Notes { get; set; }

    public ICollection<ProjectFile> Files { get; set; }

}
