#nullable disable
using TaskManager.Areas.Identity.Data;
using TaskManager.Core.Repositories;
using TaskManager.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManager.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using TaskManager.Models;

namespace TaskManager.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TaskManagerContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _accessor;

    public UserRepository(TaskManagerContext context, SignInManager<ApplicationUser> signInManager, 
        UserManager<ApplicationUser> userManager, IHttpContextAccessor accessor)
    {
        _context = context;
        _signInManager = signInManager;
        _userManager = userManager;
        _accessor = accessor;
    }

    public ICollection<ApplicationUser> GetUsers()
    {
        return _context.Users.ToList();
    }

    public async Task<ApplicationUser> GetUserAsync(string id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<ApplicationUser> GetUserWithProjectsAndTickets(string id)
    {
        var u = await _context.Users
            .Include(u => u.Projects)
                .ThenInclude(p => p.Project)
                    .ThenInclude(x => x.Tickets)
            .Include(u => u.Tickets)
                .ThenInclude(t => t.Ticket)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        return u;
    }

    public ApplicationUser UpdateUser(ApplicationUser user)
    {
        _context.Update(user);
        //_context.SaveChanges();
        return user;
    }

    public async Task<IList<string>> GetUserRoles(string id)
    {
        var user = await _context.Users.FindAsync(id);
        return await _signInManager.UserManager.GetRolesAsync(user);
    }

    public async Task<ApplicationUser> GetCurrentUser()
    {
        var currentUser = await _userManager.GetUserAsync(_accessor?.HttpContext.User);
        return currentUser;
    }

    public async Task<ICollection<ProjectAssignment>> GetProjectsForUser(string userId)
    {
        return await _context.ProjectAssignments
            .Where(pa => pa.ApplicationUser.Id == userId)
            .Include(p => p.Project)
                .ThenInclude(x=>x.Tickets)
            .Include(u=>u.ApplicationUser)
            .AsNoTracking()
            .ToListAsync();
    }

}
