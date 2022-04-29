using TaskManager.Areas.Identity.Data;
using TaskManager.Models;

namespace TaskManager.Core.Repositories;

public interface IUserRepository
{
    ICollection<ApplicationUser> GetUsers();
    Task<ApplicationUser> GetUserAsync(string id);
    Task<ApplicationUser> GetUserWithProjectsAndTickets(string id);
    ApplicationUser UpdateUser(ApplicationUser user);
    Task<IList<string>> GetUserRoles(string id);
    Task<ApplicationUser> GetCurrentUser();
    Task<ICollection<ProjectAssignment>> GetProjectsForUser(string userId);

}
