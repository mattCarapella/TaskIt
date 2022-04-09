using TaskManager.Models;

namespace TaskManager.Core.Repositories;

public interface ITicketAssignmentRepository
{
    Task<TicketAssignment> GetTicketAssignment(Guid paId, bool tracking = true);
    Task<TicketAssignment> GetTicketAssignmentForUser(string userId);

    Task<List<TicketAssignment>> GetTicketAssignmentsForUser(string userId);
    Task<List<TicketAssignment>> GetTicketAssignmentsWithProjectForUser(string userId);

    Task<List<TicketAssignment>> GetClosedTicketAssignmentsWithProjectForUser(string userId);

    Task AddTicketAssignment(TicketAssignment ta);
    Task DeleteTicketAssignment(Guid taId);
}
