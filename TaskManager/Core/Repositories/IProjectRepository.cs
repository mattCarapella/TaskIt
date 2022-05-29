using TaskManager.Models;

namespace TaskManager.Core.Repositories;

public interface IProjectRepository
{
    int GetProjectCountForUser(string userId);
    Task<Project> GetProject(Guid projectId, bool tracking=true);
    Task<Project> GetProjectWithUsers(Guid projectId);
    Task<Project> GetProjectWithTicketsNotesUsers(Guid projectId);
    Task<IEnumerable<Project>> GetProjectsWithTickets();
    Task<IEnumerable<Project>> GetProjectsWithTicketsForUser(string userId);
    Task<List<Project>> GetProjectsForManager(string userId);
    Task<int> GetProjectCountForManager(string userId);
    Task AddProject(Project project);
    Task DeleteProject(Guid projectId);
}