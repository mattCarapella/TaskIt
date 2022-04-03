using TaskManager.Areas.Identity.Data;

namespace TaskManager.Core.Repositories;

public interface IUserRepository
{
    ICollection<ApplicationUser> GetUsers();

    ApplicationUser GetUser(string id);
    Task<ApplicationUser> GetUserAsync(string id);

    Task<ApplicationUser> GetUserWithProjects(string id);

    ApplicationUser UpdateUser(ApplicationUser user);


}
