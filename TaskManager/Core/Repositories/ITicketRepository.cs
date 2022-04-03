using TaskManager.Models;
using TaskManager.Repositories;

namespace TaskManager.Core.Repositories;

public interface ITicketRepository
{
   

    Task<Ticket> GetTicket(Guid id);
    Task<Ticket> GetTicketWithProject(Guid id);
    Task<Ticket> GetTicketWithAssignedUsers(Guid id);
    Task<ICollection<Ticket>> GetTicketsAssignedToUser(Guid ticketId, string userId);
    Task<ICollection<Ticket>> GetTickets();
    Task<List<Ticket>> GetTicketsWithProjects();
    Task AddTicket(Ticket ticket);
    Task DeleteTicket(Guid id);

}
