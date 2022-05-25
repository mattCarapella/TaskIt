using TaskManager.Models;

namespace TaskManager.Core.Repositories;

public interface IProjectAssignmentRepository
{
    int GetUserProjectCount(string userId);

    Task<ProjectAssignment> GetProjectAssignment(Guid paId, bool tracking=true);
    Task<ProjectAssignment> GetProjectAssignmentForUser(string userId);

    Task<List<ProjectAssignment>> GetProjectAssignmentsForUser(string userId);
    Task<List<ProjectAssignment>> GetProjectAssignmentsWithTicketsForUser(string userId);

    Task AddProjectAssignment(ProjectAssignment pa);
    Task DeleteProjectAssignment(Guid paId);
}
