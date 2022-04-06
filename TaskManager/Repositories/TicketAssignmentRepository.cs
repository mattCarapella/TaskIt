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

    public async Task<TicketAssignment> GetTicketAssignment(Guid taId, bool tracking = true)
    {
        if (!tracking)
        {
            return await _context.TicketAssignments.AsNoTracking().FirstOrDefaultAsync(t => t.TicketAssignmentId == taId);
        }
        return await _context.TicketAssignments.FindAsync(taId);
    }
    public async Task<TicketAssignment> GetTicketAssignmentForUser(string userId)
    {
        return await _context.TicketAssignments.FirstOrDefaultAsync(ta => ta.ApplicationUserId == userId);

    }

    public async Task<List<TicketAssignment>> GetTicketAssignmentsForUser(string userId)
    {
        return await _context.TicketAssignments
            .Where(u => u.ApplicationUserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<TicketAssignment>> GetTicketAssignmentsWithProjectForUser(string userId)
    {
        return await _context.TicketAssignments
                    .Where(u => u.ApplicationUserId == userId)
                    .Include(t => t.Ticket)
                        .ThenInclude(p => p.Project)
                    .AsNoTracking()
                    .ToListAsync();
    }

    public async Task AddTicketAssignment(TicketAssignment ta)
    {
        await _context.TicketAssignments.AddAsync(ta);
    }

    public async Task DeleteTicketAssignment(Guid taId)
    {
        var ticketAssignment = await _context.TicketAssignments.FirstOrDefaultAsync(p => p.TicketAssignmentId == taId);
        _context.TicketAssignments.Remove(ticketAssignment);
    }
}
