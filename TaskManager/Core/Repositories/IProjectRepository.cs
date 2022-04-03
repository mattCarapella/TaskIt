using TaskManager.Models;

namespace TaskManager.Core.Repositories;

public interface IProjectRepository
{
    Task<Project> GetProject(Guid projectId);
    Task<Project> GetProjectWithUsers(Guid projectId);
    Task<Project> GetProjectWithTicketsNotesUsers(Guid projectId);
    Task<ICollection<Project>> GetProjects();
    Task<List<Project>> GetProjectsWithTickets();
    Task AddProject(Project project);
    Task DeleteProject(Guid projectId);
}