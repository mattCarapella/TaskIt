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

    public async Task AddProjectAssignment(ProjectAssignment pa)
    {
        await _context.ProjectAssignments.AddAsync(pa);
    }
    
    public async Task DeleteProjectAssignment(Guid paId)
    {
        var projectAssignment = await _context.ProjectAssignments.FirstOrDefaultAsync(p => p.ProjectAssignmentId == paId);
        _context.ProjectAssignments.Remove(projectAssignment!);
    }

}
