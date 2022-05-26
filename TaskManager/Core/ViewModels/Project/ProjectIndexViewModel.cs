namespace TaskManager.Core.ViewModels.Project;

public class ProjectIndexViewModel
{
    public PaginatedList<Models.Project> Projects { get; set; }
    
    public int OpenTicketCount;

}
