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
    public List<Models.Ticket>? UpcomingDeadlines { get; set; }
    public string? CurrentUserName { get; set; }
    public List<int>? PriorityCounts { get; set; }
    public List<int>? TypeCounts { get; set; }
    public List<string>? PriorityKeys { get; set; }
    public List<string>? TypeKeys { get; set; }
    public int? ManagerProjectCount { get; set; }
    public List<Models.Ticket>? TicketsToAssign { get; set; }
    public List<Models.Ticket>? TicketsToReview { get; set; }
    public int? AssignCount { get; set; }
    public int? ReviewCount { get; set; }
}
