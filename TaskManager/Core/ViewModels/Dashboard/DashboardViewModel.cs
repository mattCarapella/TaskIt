using TaskManager.Core.ViewModels.Dashboard;
using TaskManager.Models;

namespace TaskManager.Core.ViewModels;

public class DashboardViewModel
{
    public int ProjectCount { get; set; }
    public int OpenTicketCount { get; set; }
    public int ClosedTicketCount { get; set; }


    public IEnumerable<TicketPriorityGroup>? TicketPriorityGroup { get; set; }
    public IEnumerable<TicketStatusGroup>? TicketStatusGroup { get; set; }
    public IEnumerable<TicketTypeGroup>? TicketTypeGroup { get; set; }
    public IEnumerable<Models.Ticket>? UpcomingDeadlines { get; set; }


    public string? CurrentUserName { get; set; }
    public List<int>? PriorityCounts { get; set; }
    public List<int>? TypeCounts { get; set; }


    public List<Models.Project>? ManagerProjects { get; set; }
    public List<Models.Ticket>? ManagerTicketsToAssign { get; set; }
    public List<Models.Ticket>? ManagerTicketsToReview { get; set; }


}
