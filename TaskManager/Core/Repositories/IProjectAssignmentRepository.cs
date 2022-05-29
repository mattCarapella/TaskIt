using TaskManager.Models;

namespace TaskManager.Core.Repositories;

public interface IProjectAssignmentRepository
{
    Task AddProjectAssignment(ProjectAssignment pa);
    Task DeleteProjectAssignment(Guid paId);
}

