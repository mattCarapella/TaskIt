using Microsoft.AspNetCore.Identity;
using TaskManager.Areas.Identity.Data;
using TaskManager.Core.Repositories;
using TaskManager.Data;

namespace TaskManager.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly TaskManagerContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RoleRepository(TaskManagerContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        public ICollection<IdentityRole> GetRoles()
        {
            return _context.Roles.ToList();
        }
    }
}
