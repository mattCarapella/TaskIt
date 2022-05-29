using TaskManager.Models;

namespace TaskManager.Core.Repositories;

public interface ITicketAssignmentRepository
{
    Task<List<TicketAssignment>> GetTicketAssignmentsWithProjectForUser(string userId);
    Task<List<TicketAssignment>> GetClosedTicketAssignmentsWithProjectForUser(string userId);
    Task AddTicketAssignment(TicketAssignment ta);
    Task DeleteTicketAssignment(Guid taId);
}
