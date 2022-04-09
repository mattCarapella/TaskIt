using TaskManager.Models;

namespace TaskManager.Core.ViewModels.Project;

public class ProjectDetailsViewModel
{
    public TaskManager.Models.Project Project { get; set; }

    public ICollection<ProjectAssignment> Contributers { get; set; }

    public ICollection<Models.Ticket> OpenTickets { get; set; }

    public ICollection<Models.Ticket> ClosedTickets   { get; set; }

    public ICollection<Models.Ticket> AllOpenTickets { get; set; }

    public ICollection<PNote> Notes { get; set; }

}
