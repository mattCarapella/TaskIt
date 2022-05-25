using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Repositories;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Repositories;

public class TicketAssignmentRepository : ITicketAssignmentRepository
{
    private readonly TaskManagerContext _context;

    public TicketAssignmentRepository(TaskManagerContext context)
    {
        _context = context;
    }

    public int GetUserOpenTicketCount(string userId)
    {
        return _context.TicketAssignments
                .Where(u => u.ApplicationUserId == userId)
                .Where(t => t.Ticket!.Status != Core.Enums.Enums.Status.COMPLETED)
                .Include(t => t.Ticket)
                .AsNoTracking()
                .Count();
    }

    public int GetUserClosedTicketCount(string userId)
    {
        return _context.TicketAssignments
                .Where(u => u.ApplicationUserId == userId)
                .Where(t => t.Ticket!.Status == Core.Enums.Enums.Status.COMPLETED)
                .Include(t => t.Ticket)
                .AsNoTracking()
                .Count();
    }

    public async Task<List<TicketAssignment>> GetTicketAssignmentsForUser(string userId)
    {
        return await _context.TicketAssignments
            .Where(u => u.ApplicationUserId == userId)
            .Include(t => t.Ticket)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<TicketAssignment>> GetTicketAssignmentsWithProjectForUser(string userId)
    {
        return await _context.TicketAssignments
                    .AsNoTracking()
                    .Where(u => u.ApplicationUserId == userId)
                    .Where(t => t.Ticket!.Status != Core.Enums.Enums.Status.COMPLETED)
                    .Include(t => t.Ticket)
                        .ThenInclude(p => p!.Project)
                    .ToListAsync();
    }

    public async Task<List<TicketAssignment>> GetClosedTicketAssignmentsWithProjectForUser(string userId)
    {
        return await _context.TicketAssignments
                    .AsNoTracking()
                    .Where(u => u.ApplicationUserId == userId)
                    .Where(t => t.Ticket!.Status == Core.Enums.Enums.Status.COMPLETED)
                    .Include(t => t.Ticket)
                        .ThenInclude(p => p!.Project)
                    .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetUpcomingTicketDeadlines(string userId)
    {
        var ta = await _context.TicketAssignments
                       .Where(u => u.ApplicationUserId == userId)
                       .Where(t => t.Ticket!.Status != Core.Enums.Enums.Status.COMPLETED)
                       .Where(t => t.Ticket!.GoalDate > DateTime.Now && t.Ticket!.GoalDate < DateTime.Now.AddDays(14))
                       .Take(5)
                       .Include(t => t.Ticket)
                       .OrderBy(t => t.Ticket!.GoalDate)
                       .AsNoTracking()
                       .ToListAsync();

        var tickets = from t in ta select t.Ticket;
        
        return tickets;
    }

    public async Task AddTicketAssignment(TicketAssignment ta)
    {
        await _context.TicketAssignments.AddAsync(ta);
    }

    public async Task DeleteTicketAssignment(Guid taId)
    {
        var ticketAssignment = await _context.TicketAssignments.FirstOrDefaultAsync(p => p.TicketAssignmentId == taId);
        if (ticketAssignment is not null)
        {
            _context.TicketAssignments.Remove(ticketAssignment);
        }
    }



    //public async Task<TicketAssignment> GetTicketAssignment(Guid taId, bool tracking = true)
    //{
    //    if (!tracking)
    //    {
    //        return await _context.TicketAssignments.AsNoTracking().FirstOrDefaultAsync(t => t.TicketAssignmentId == taId);
    //    }
    //    return await _context.TicketAssignments.FindAsync(taId);
    //}
    //public async Task<TicketAssignment> GetTicketAssignmentForUser(string userId)
    //{
    //    return await _context.TicketAssignments.FirstOrDefaultAsync(ta => ta.ApplicationUserId == userId);

    //}

}
