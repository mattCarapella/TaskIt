using Microsoft.AspNetCore.Identity;

namespace TaskManager.Core.Repositories;

public interface IRoleRepository
{
    ICollection<IdentityRole> GetRoles();
}