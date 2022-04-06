using Microsoft.AspNetCore.Identity;
using TaskManager.Areas.Identity.Data;
using TaskManager.Core.Repositories;
using TaskManager.Data;

namespace TaskManager.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly TaskManagerContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UnitOfWork(TaskManagerContext context, SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _signInManager = signInManager;
    }

    public IUserRepository UserRepository => new UserRepository(_context, _signInManager);
    public IRoleRepository RoleRepository => new RoleRepository(_context, _signInManager);
    public IProjectRepository ProjectRepository => new ProjectRepository(_context);
    public ITicketRepository TicketRepository => new TicketRepository(_context);
    public IProjectAssignmentRepository ProjectAssignmentRepository => new ProjectAssignmentRepository(_context);   

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

}








    //public IUserRepository User { get;  }
    //public IRoleRepository Role { get; }

    //public UnitOfWork(IUserRepository user, IRoleRepository role)//, ITicketRepository ticket)
    //{
    //    User = user;
    //    Role = role;
    //}


