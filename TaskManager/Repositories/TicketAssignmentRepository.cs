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
}
