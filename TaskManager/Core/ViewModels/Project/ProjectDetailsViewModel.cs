using TaskManager.Models;

namespace TaskManager.Core.ViewModels.Project;

public class ProjectDetailsViewModel
{
    public TaskManager.Models.Project Project { get; set; }

    public List<ProjectAssignment> Contributers { get; set; }

    public List<Ticket> OpenTickets { get; set; }

    public List<Ticket> ClosedTickets   { get; set; }

}
