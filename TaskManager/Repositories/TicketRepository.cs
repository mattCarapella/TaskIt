using TaskManager.Core.Repositories;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly TaskManagerContext _context;

    public TicketRepository(TaskManagerContext context)
    {
        _context = context;
    }

    public ICollection<Ticket> GetTickets()
    {
        return _context.Tickets.ToList();
    }

    //public async Task<Ticket> GetTicket(Guid? id)
    //{
    //    return await _context.Tickets.FindAsync(id);
    //}

}
