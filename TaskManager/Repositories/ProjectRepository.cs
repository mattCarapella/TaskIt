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

    public Project GetProject(Guid projectId)
    {
        return _context.Projects.Find(projectId);
    }

    public async Task<Project> GetProjectAsync(Guid projectId)
    {
        return await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<Project> GetProjectWithUsers(Guid projectId)
    {
        return await _context.Projects
                .Include(c => c.Contributers)
                .ThenInclude(u => u.ApplicationUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == projectId);
    }

    public async Task<Project> GetProjectWithTicketsNotesUsers(Guid projectId)
    {
        return await _context.Projects
                .Include(t => t.Tickets)
                .Include(n => n.Notes)
                .ThenInclude(u => u.ApplicationUser)
                .Include(c => c.Contributers)
                .ThenInclude(u => u.ApplicationUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == projectId);
    }

    public ICollection<Project> GetProjects()
    {
        return _context.Projects.ToList();
    }

    public IQueryable<Project> GetProjectsWithTickets()
    {
        return _context.Projects.Include(p => p.Tickets);
    }

    public void AddProject(Project project)
    {
        _context.Projects.Add(project);
    }

    public void UpdateProject(Project project)
    {
        _context.Projects.Update(project);
    }

    public void DeleteProject(Guid projectId)
    {
        var project = _context.Projects.Find(projectId);
        _context.Projects.Remove(project);
    }
}
