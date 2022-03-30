using TaskManager.Models;
using TaskManager.Repositories;

namespace TaskManager.Core.Repositories;

public interface ITicketRepository
{
    ICollection<Ticket> GetTickets();

    //Task<Ticket> GetTicket(Guid? id);

}
