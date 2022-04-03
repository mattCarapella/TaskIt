using Microsoft.AspNetCore.Identity;
using TaskManager.Core.Repositories;
using TaskManager.Data;

namespace TaskManager.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly TaskManagerContext _context;

        public RoleRepository(TaskManagerContext context)
        {
            _context = context;
        }

        public ICollection<IdentityRole> GetRoles()
        {
            return _context.Roles.ToList();
        }
    }
}
