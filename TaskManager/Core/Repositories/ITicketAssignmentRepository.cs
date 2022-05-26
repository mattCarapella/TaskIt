using TaskManager.Models;

namespace TaskManager.Core.Repositories;

public interface ITicketAssignmentRepository
{
    int GetUserOpenTicketCount(string userId);
    int GetUserClosedTicketCount(string userId);
    
    Task<List<TicketAssignment>> GetTicketAssignmentsForUser(string userId);
    Task<List<TicketAssignment>> GetTicketAssignmentsWithProjectForUser(string userId);
    Task<List<TicketAssignment>> GetClosedTicketAssignmentsWithProjectForUser(string userId);
    Task<List<Ticket>> GetUpcomingTicketDeadlines(string userId);

    Task AddTicketAssignment(TicketAssignment ta);
    Task DeleteTicketAssignment(Guid taId);
}
