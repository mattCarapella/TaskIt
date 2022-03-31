using TaskManager.Areas.Identity.Data;
using TaskManager.Core.Repositories;
using TaskManager.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

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

    public async Task<ApplicationUser> GetUserWithProjects(string id)
    {
        var user = await _context.Users
            .Include(u => u.Projects)
            .ThenInclude(p => p.Project)
            .Include(t => t.Tickets)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return user;
    }


    public ApplicationUser UpdateUser(ApplicationUser user)
    {
        _context.Update(user);
        _context.SaveChanges();
        return user;
    }


}
