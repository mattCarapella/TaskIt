#nullable disable
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Repositories;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly TaskManagerContext _context;

    public ProjectRepository(TaskManagerContext context)
    {
        _context = context;
    }

    public async Task<Project> GetProject(Guid projectId, bool tracking=true)
    {
        if (!tracking)
        {
            return await _context.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId);
        }
        return await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
    }


    public async Task<Project> GetProjectWithUsers(Guid projectId)
    {
        return await _context.Projects
                .Include(c => c.Contributers)
                    .ThenInclude(u => u.ApplicationUser)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == projectId);
    }


    public async Task<Project> GetProjectWithTicketsNotesUsers(Guid projectId)
    {
        return await _context.Projects
                .Include(c => c.Contributers)
                    .ThenInclude(u => u.ApplicationUser)
                .Include(t => t.Tickets)
                    .ThenInclude(a => a.AssignedTo)
                        .ThenInclude(u=>u.ApplicationUser)
                .Include(n => n.Notes)
                    .ThenInclude(u => u.ApplicationUser)
                .Include(p => p.CreatedByUser)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == projectId);
    }


    public async Task<List<Project>> GetProjectsForManager(string userId)
    {
        var user = _context.Users.Find(userId);
        return await _context.Projects
            .Where(p => p.CreatedByUser == user)
            .Include(p => p.Tickets)
            .AsNoTracking()
            .ToListAsync();
    }


    public async Task<List<Project>> GetProjectsWithTickets()
    {
        return await _context.Projects
            .Include(p => p.Tickets)
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync();
    }


    public async Task AddProject(Project project)
    {
        await _context.Projects.AddAsync(project);
    }


    public async Task DeleteProject(Guid projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        _context.Projects.Remove(project);
    }
}
