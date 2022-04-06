using TaskManager.Models;

namespace TaskManager.Core.ViewModels.Project;

public class ProjectDetailsViewModel
{
    public TaskManager.Models.Project Project { get; set; }

    public List<ProjectAssignment> Contributers { get; set; }

    public List<Models.Ticket> OpenTickets { get; set; }

    public List<Models.Ticket> ClosedTickets   { get; set; }

    public List<Models.Ticket> AllOpenTickets { get; set; }

    public List<PNote> Notes { get; set; }

}
