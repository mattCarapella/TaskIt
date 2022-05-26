using TaskManager.Models;

namespace TaskManager.Core.Repositories;

public interface IProjectRepository
{
    Task<Project> GetProject(Guid projectId, bool tracking=true);
    Task<Project> GetProjectWithUsers(Guid projectId);
    Task<Project> GetProjectWithTicketsNotesUsers(Guid projectId);
    Task<List<Project>> GetProjectsWithTickets();
    Task<List<Project>> GetProjectsForManager(string userId);

    Task AddProject(Project project);
    Task DeleteProject(Guid projectId);
}