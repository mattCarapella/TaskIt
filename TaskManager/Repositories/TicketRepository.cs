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

    public IDictionary<string, int> GetTicketCounts(IEnumerable<Ticket> userTickets, string userId)
    {
        var open = userTickets.Where(t => t.Status != Core.Enums.Enums.Status.COMPLETED).Count();
        var closed = userTickets.Where(t => t.Status == Core.Enums.Enums.Status.COMPLETED).Count();
        var tickets = new Dictionary<string, int>();
        tickets.Add("OPEN", open);
        tickets.Add("CLOSED", closed);
        return tickets;
    }

    public async Task<Ticket> GetTicket(Guid ticketId, bool tracking=true)
    {
        if (!tracking)
        {
            return await _context.Tickets.AsNoTracking().FirstOrDefaultAsync(t => t.TicketId == ticketId);
        }
        return await _context.Tickets.FindAsync(ticketId);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsAssignedToUser(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return await _context.TicketAssignments
                .Where(u => u.ApplicationUserId == userId)
                .Select(t => t.Ticket)
                .Include(t => t.Project)
                .AsNoTracking()
                .ToListAsync();
    }


    public async Task<Ticket> GetTicketWithProjectAndUserDetails(Guid ticketId)
    {
        return await _context.Tickets
                .Include(p => p.Project)
                .Include(s => s.SubmittedBy)
                .Include(c => c.AssignedTo)
                    .ThenInclude(u => u.ApplicationUser)
                .Include(n => n.TNotes)
                    .ThenInclude(u => u.ApplicationUser)
                .Include(t => t.TicketFiles)
                    .ThenInclude(u => u.UploadedByUser)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);
    }


    public async Task<List<Ticket>> GetTicketsWithProjects()
    {
        return await _context.Tickets
                        .AsNoTracking()
                        .Where(t => t.Status == Core.Enums.Enums.Status.INPROGRESS)
                        .Include(p => p.Project)
                        .ToListAsync();
    }


    public async Task<List<Ticket>> GetClosedTicketsWithProjects()
    {
        return await _context.Tickets
                        .AsNoTracking()
                        .Where(t => t.Status == Core.Enums.Enums.Status.COMPLETED)
                        .Include(p => p.Project)
                        .ToListAsync();
    }


    public async Task<IEnumerable<Ticket>> GetClosedTicketsWithProjectsForUser(string userId)
    {
        return await _context.TicketAssignments
                    .AsNoTracking()
                    .Where(u => u.ApplicationUserId == userId)
                    .Where(t => t.Ticket!.Status == Core.Enums.Enums.Status.COMPLETED)
                    .Select(t => t.Ticket)
                        .Include(t => t!.Project)
                    .ToListAsync();
    }



    public async Task<List<Ticket>> GetTicketsForManagersProjects(string userId)
    {
        var user = _context.Users.Find(userId);
        return await _context.Tickets
                            .Include(t => t.Project)
                            .Where(t => t.Project.CreatedByUserId == userId)
                            .AsNoTracking()
                            .ToListAsync();
    }


    public async Task<List<Ticket>> GetTicketsToAssign()
    {
        return await _context.Tickets
                        .Include(p => p.Project)
                        .Where(t => t.Status == Core.Enums.Enums.Status.TODO)
                        .AsNoTracking()
                        .ToListAsync();
    }


    public async Task<List<Ticket>> GetTicketsToAssignForManager(string userId)
    {
        var user = _context.Users.Find(userId);
        return await _context.Tickets
                        .Include(t => t.Project)
                        .Where(t => t.Project.CreatedByUserId == userId)
                        .Where(t => t.Status == Core.Enums.Enums.Status.TODO)
                        .AsNoTracking()
                        .ToListAsync();
    }


    public async Task<List<Ticket>> GetTicketsForReview()
    {
        return await _context.Tickets
                        .Where(t => t.Status == Core.Enums.Enums.Status.SUBMITTED)
                        .Include(p => p.Project)
                        .AsNoTracking()
                        .ToListAsync();
    }

    public async Task<List<Ticket>> GetTicketsForReviewForManager(string userId)
    {
        var user = _context.Users.Find(userId);
        return await _context.Tickets
                        .Include(t => t.Project)
                        .Where(t => t.Project.CreatedByUserId == userId)
                        .Where(t => t.Status == Core.Enums.Enums.Status.SUBMITTED)
                        .AsNoTracking()
                        .ToListAsync();
    }


    public async Task<List<Ticket>> GetTicketsForProjectDetailsPage(Guid projectId, string userId)
    {
        var currentUser = _context.Users.Find(userId);
        return await _context.TicketAssignments
                     .Where(t => t.ApplicationUser == currentUser)
                     .Where(t => t.Ticket.ProjectId == projectId)
                     .Select(t => t.Ticket)
                     .AsNoTracking()
                     .ToListAsync();
    }


    /*******************************************************************************/
    /*                                   DASHBOARD                                 */
    /*******************************************************************************/

    public List<Ticket> GetUpcomingTicketDeadlines(IEnumerable<Ticket> userTickets)
    {
        return userTickets.Where(t => t.Status != Core.Enums.Enums.Status.COMPLETED)
                          .Where(t => t.GoalDate > DateTime.Now && t.GoalDate < DateTime.Now.AddDays(14))
                          .Take(5)
                          .OrderBy(t => t.GoalDate)
                          .ToList();
    }


    public async Task<IDictionary<string, List<Ticket>>> GetTicketDetailsForUserDashboard(string userId)
    {
        var tickets = await _context.TicketAssignments
                            .Where(u => u.ApplicationUserId == userId)
                            .Select(t => t.Ticket)
                            .AsNoTracking()
                            .ToListAsync();

        var deadlines = tickets.Where(t => t.Status != Core.Enums.Enums.Status.COMPLETED)
                               .Where(t => t.GoalDate > DateTime.Now && t.GoalDate < DateTime.Now.AddDays(14))
                               .Take(5)
                               .OrderBy(t => t.GoalDate)
                               .ToList();

        var ticketDict = new Dictionary<string, List<Ticket>>();
        ticketDict.Add("USER_TICKETS", tickets);
        ticketDict.Add("DEADLINES", deadlines);
        return ticketDict;
    }


    public async Task<IDictionary<string, List<Ticket>>> GetTicketDetailsForManagerDashboard(string userId)
    {
        var user = _context.Users.Find(userId);
        var tickets = await _context.Tickets
                            .Where(t => t.Project.CreatedByUserId == userId)
                            .AsNoTracking()
                            .ToListAsync();

        var toAssign = tickets.Where(t => t.Status == Core.Enums.Enums.Status.TODO).Take(5);
        var toReview = tickets.Where(t => t.Status == Core.Enums.Enums.Status.SUBMITTED).Take(5);
        var ticketDict = new Dictionary<string, List<Ticket>>();
        ticketDict.Add("TO_ASSIGN", toAssign.ToList());
        ticketDict.Add("TO_REVIEW", toReview.ToList());
        return ticketDict;
    }

    public async Task<IDictionary<string, int>> GetTicketDetailsCountForManagerDashboard(string userId)
    {
        var user = _context.Users.Find(userId);
        var tickets = await _context.Tickets
                            .Where(t => t.Project.CreatedByUserId == userId)
                            .AsNoTracking()
                            .ToListAsync();

        var toAssign = tickets.Where(t => t.Status == Core.Enums.Enums.Status.TODO).Count();
        var toReview = tickets.Where(t => t.Status == Core.Enums.Enums.Status.SUBMITTED).Count();
        var ticketDict = new Dictionary<string, int>();
        ticketDict.Add("TO_ASSIGN", toAssign);
        ticketDict.Add("TO_REVIEW", toReview);
        return ticketDict;
    }


    public async Task<IDictionary<string, List<Ticket>>> GetTicketDetailsForAdminDashboard(string userId)
    {
        var user = _context.Users.Find(userId);
        var tickets = await _context.Tickets
                            .AsNoTracking()
                            .ToListAsync();

        var toAssign = tickets.Where(t => t.Status == Core.Enums.Enums.Status.TODO).Take(5);
        var toReview = tickets.Where(t => t.Status == Core.Enums.Enums.Status.SUBMITTED).Take(5);
        var ticketDict = new Dictionary<string, List<Ticket>>();
        ticketDict.Add("TO_ASSIGN", toAssign.ToList());
        ticketDict.Add("TO_REVIEW", toReview.ToList());
        return ticketDict;
    }

    public async Task<IDictionary<string, int>> GetTicketDetailsCountForAdminDashboard(string userId)
    {
        var user = _context.Users.Find(userId);
        var tickets = await _context.Tickets
                             .AsNoTracking()
                             .ToListAsync();

        var toAssign = tickets.Where(t => t.Status == Core.Enums.Enums.Status.TODO).Count();
        var toReview = tickets.Where(t => t.Status == Core.Enums.Enums.Status.SUBMITTED).Count();
        var ticketDict = new Dictionary<string, int>();
        ticketDict.Add("TO_ASSIGN", toAssign);
        ticketDict.Add("TO_REVIEW", toReview);
        return ticketDict;
    }


    /*******************************************************************************/
    /*                                  CRUD                                       */
    /*******************************************************************************/
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
