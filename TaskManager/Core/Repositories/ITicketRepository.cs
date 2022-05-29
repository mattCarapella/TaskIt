using TaskManager.Models;
using TaskManager.Repositories;

namespace TaskManager.Core.Repositories;

public interface ITicketRepository
{

    IDictionary<string, int> GetTicketCounts(IEnumerable<Ticket> userTickets, string userId);
    Task<Ticket> GetTicket(Guid ticketId, bool tracking = true);
    Task<IEnumerable<Ticket>> GetTicketsAssignedToUser(string userId);
    Task<Ticket> GetTicketWithProjectAndUserDetails(Guid ticketId);
    Task<List<Ticket>> GetTicketsWithProjects();
    Task<List<Ticket>> GetClosedTicketsWithProjects();
    Task<IEnumerable<Ticket>> GetClosedTicketsWithProjectsForUser(string userId);
    Task<List<Ticket>> GetTicketsToAssign();
    Task<List<Ticket>> GetTicketsToAssignForManager(string userId);
    Task<List<Ticket>> GetTicketsForManagersProjects(string userId);
    Task<List<Ticket>> GetTicketsForReview();
    Task<List<Ticket>> GetTicketsForReviewForManager(string userId);
    Task<List<Ticket>> GetTicketsForProjectDetailsPage(Guid projectId, string userId);
    List<Ticket> GetUpcomingTicketDeadlines(IEnumerable<Ticket> userTickets);
    Task AddTicket(Ticket ticket);
    Task DeleteTicket(Guid ticketId);


    Task<IDictionary<string, List<Ticket>>> GetTicketDetailsForUserDashboard(string userId);
    Task<IDictionary<string, List<Ticket>>> GetTicketDetailsForManagerDashboard(string userId);
    Task<IDictionary<string, int>> GetTicketDetailsCountForManagerDashboard(string userId);
    Task<IDictionary<string, List<Ticket>>> GetTicketDetailsForAdminDashboard(string userId);
    Task<IDictionary<string, int>> GetTicketDetailsCountForAdminDashboard(string userId);

}

