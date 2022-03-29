using TaskManager.Areas.Identity.Data;

namespace TaskManager.Core.Repositories;

public interface IUserRepository
{
    ICollection<ApplicationUser> GetUsers();

    ApplicationUser GetUser(string id);

    ApplicationUser UpdateUser(ApplicationUser user);

}
