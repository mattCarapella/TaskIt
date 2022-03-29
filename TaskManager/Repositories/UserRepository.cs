using TaskManager.Areas.Identity.Data;
using TaskManager.Core.Repositories;
using TaskManager.Data;

namespace TaskManager.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TaskManagerContext _context;
    public UserRepository(TaskManagerContext context)
    {
        _context = context; 
    }

    public ICollection<ApplicationUser> GetUsers()
    {
        return _context.Users.ToList();
    }

    public ApplicationUser GetUser(string id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public ApplicationUser UpdateUser(ApplicationUser user)
    {
        _context.Update(user);
        _context.SaveChanges();
        return user;
    }

}
