using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Repositories;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Repositories;

public class ProjectAssignmentRepository : IProjectAssignmentRepository
{
    private readonly TaskManagerContext _context;

    public ProjectAssignmentRepository(TaskManagerContext context)
    {
        _context = context;
    }

    public int GetUserProjectCount(string userId)
    {
        return _context.ProjectAssignments
               .Where(u => u.ApplicationUserId == userId)
               .AsNoTracking()
               .Count();
    }

    public async Task<ProjectAssignment> GetProjectAssignment(Guid paId, bool tracking=true)
    {
        if (!tracking)
        {
            return await _context.ProjectAssignments.AsNoTracking().FirstOrDefaultAsync(p => p.ProjectAssignmentId == paId);
        }
        return await _context.ProjectAssignments.FindAsync(paId);
    }

    public async Task<ProjectAssignment> GetProjectAssignmentForUser(string userId)
    {
        //if (!tracking)
        //{
        //    return await _context.ProjectAssignments.AsNoTracking().FirstOrDefaultAsync(p => p.ProjectAssignmentId == paId);
        //}
        return await _context.ProjectAssignments.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
    }

    public async Task<List<ProjectAssignment>> GetProjectAssignmentsForUser(string userId)
    {
        return await _context.ProjectAssignments
            .Where(u => u.ApplicationUserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<ProjectAssignment>> GetProjectAssignmentsWithTicketsForUser(string userId)
    {
        return await _context.ProjectAssignments
            .Where(u => u.ApplicationUserId == userId)
            .Include(p => p.Project)
                .ThenInclude(t => t.Tickets)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddProjectAssignment(ProjectAssignment pa)
    {
        await _context.ProjectAssignments.AddAsync(pa);
    }
    
    public async Task DeleteProjectAssignment(Guid paId)
    {
        var projectAssignment = await _context.ProjectAssignments.FirstOrDefaultAsync(p => p.ProjectAssignmentId == paId);
        _context.ProjectAssignments.Remove(projectAssignment);
    }

}
