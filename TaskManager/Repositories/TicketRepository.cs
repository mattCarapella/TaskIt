#nullable disable
using Microsoft.EntityFrameworkCore;
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

    public async Task<Ticket> GetTicket(Guid id)
    {
        return await _context.Tickets.FindAsync(id);
    }


    public async Task<Ticket> GetTicketWithProject(Guid id)
    {
        return await _context.Tickets
                        .Include(p => p.Project)
                        .FirstOrDefaultAsync(x => x.TicketId == id);
    }


    public async Task<Ticket> GetTicketWithAssignedUsers(Guid id)
    {
        return await _context.Tickets
                        .Include(a => a.AssignedTo)
                        .ThenInclude(u => u.ApplicationUser)
                        .FirstOrDefaultAsync(x => x.TicketId == id);
    }


    public async Task<ICollection<Ticket>> GetTickets()
    {
        return await _context.Tickets.ToListAsync();
    }


    public async Task<List<Ticket>> GetTicketsWithProjects()
    {
        return await _context.Tickets
                        .Include(p => p.Project)
                        .ToListAsync();
    }


    public async Task<ICollection<Ticket>> GetTicketsAssignedToUser(Guid ticketId, string userId)
    {
        var user = _context.Users.Find(userId);
        var tickets = await _context.Tickets
                               .Include(t => t.AssignedTo)
                               .ThenInclude(x => x.ApplicationUser)
                               .ToListAsync();
        return tickets;
    }


    public async Task AddTicket(Ticket ticket)
    {
        await _context.Tickets.AddAsync(ticket);
    }


    public async Task DeleteTicket(Guid id)
    {
        var ticket = await _context.Tickets.FindAsync(id); 
        _context.Tickets.Remove(ticket);
    }


}