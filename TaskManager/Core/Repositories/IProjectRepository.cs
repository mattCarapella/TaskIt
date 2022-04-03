using TaskManager.Models;

namespace TaskManager.Core.Repositories;

public interface IProjectRepository
{
    Project GetProject(Guid projectId);
    Task<Project> GetProjectAsync(Guid projectId);
    Task<Project> GetProjectWithUsers(Guid projectId);
    Task<Project> GetProjectWithTicketsNotesUsers(Guid projectId);

    ICollection<Project> GetProjects();
    IQueryable<Project> GetProjectsWithTickets();

    void AddProject(Project project);

    void UpdateProject(Project project);

    void DeleteProject(Guid projectId);
}
