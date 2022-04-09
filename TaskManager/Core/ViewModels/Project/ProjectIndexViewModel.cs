namespace TaskManager.Core.ViewModels.Project;

public class ProjectIndexViewModel
{
    public ICollection<Models.Project> Projects { get; set; }

    public int OpenTicketCount;

}
