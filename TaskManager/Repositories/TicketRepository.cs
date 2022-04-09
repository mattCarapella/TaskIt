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

    public async Task<Ticket> GetTicket(Guid ticketId, bool tracking=true)
    {
        if (!tracking)
        {
            return await _context.Tickets.AsNoTracking().FirstOrDefaultAsync(t => t.TicketId == ticketId);
        }
        return await _context.Tickets.FindAsync(ticketId);
    }


    public async Task<Ticket> GetTicketWithProject(Guid ticketId)
    {
        return await _context.Tickets
                        .Include(p => p.Project.Id)
                        .FirstOrDefaultAsync(x => x.TicketId == ticketId);
    }


    public async Task<Ticket> GetTicketWithAssignedUsers(Guid ticketId)
    {
        return await _context.Tickets
                        .Include(a => a.AssignedTo)
                        .ThenInclude(u => u.ApplicationUser)
                        .FirstOrDefaultAsync(x => x.TicketId == ticketId);
    }


    //public async Task<Ticket> GetTicketToAssignUser(Guid id)
    public async Task<Ticket> GetTicketWithProjectAndUserDetails(Guid ticketId)
    {
        return await _context.Tickets
                .Include(p => p.Project)
                .Include(s => s.SubmittedBy)
                .Include(c => c.AssignedTo)
                    .ThenInclude(u => u.ApplicationUser)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);
    }




    public async Task<ICollection<Ticket>> GetTickets()
    {
        return await _context.Tickets
                        .AsNoTracking()
                        .ToListAsync();
    }


    public async Task<List<Ticket>> GetTicketsWithProjects()
    {
        return await _context.Tickets
                        .AsNoTracking()
                        .Include(p => p.Project)
                        .ToListAsync();
    }

    public async Task<List<Ticket>> GetTicketsForReview()
    {
        return await _context.Tickets
                        .Where(t=>t.Status == Core.Enums.Enums.Status.SUBMITTED)
                        .Include(p => p.Project)
                        .AsNoTracking()
                        .ToListAsync();
    }


    public async Task<List<Ticket>> GetTicketsAssignedToUser(string userId)
    {
        var user = _context.Users.Find(userId);
        var tickets = await _context.Tickets
                            .Include(t => t.Project)
                            .Include(u => u.AssignedTo)
                                .ThenInclude(a => a.ApplicationUser)
                             .AsNoTracking()
                            .ToListAsync();

                               //.Include(t => t.AssignedTo 
                               //     .Where(u => u.ApplicationUserId == userId))
                               //     .ThenInclude(x => x.ApplicationUser)
                               //.OrderBy(u => u.Title)
                               //.ToListAsync();
        return tickets;
    }


    public async Task AddTicket(Ticket ticket)
    {
        await _context.Tickets.AddAsync(ticket);
    }


    public async Task DeleteTicket(Guid ticketId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId); 
        _context.Tickets.Remove(ticket);
    }


}