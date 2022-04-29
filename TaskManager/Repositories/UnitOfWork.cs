using Microsoft.AspNetCore.Identity;
using TaskManager.Areas.Identity.Data;
using TaskManager.Core.Repositories;
using TaskManager.Data;

namespace TaskManager.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly TaskManagerContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _accessor;

    public UnitOfWork(TaskManagerContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHttpContextAccessor accessor)
    {
        _context = context;
        _signInManager = signInManager;
        _userManager = userManager;
        _accessor = accessor;
    }

    public IUserRepository UserRepository => new UserRepository(_context, _signInManager, _userManager, _accessor);
    public IRoleRepository RoleRepository => new RoleRepository(_context, _signInManager);
    public IProjectRepository ProjectRepository => new ProjectRepository(_context);
    public ITicketRepository TicketRepository => new TicketRepository(_context);
    public IProjectAssignmentRepository ProjectAssignmentRepository => new ProjectAssignmentRepository(_context);   
    public ITicketAssignmentRepository TicketAssignmentRepository => new TicketAssignmentRepository(_context);
    public ITNoteRepository TNoteRepository => new TNoteRepository(_context);
    public IPNoteRepository PNoteRepository => new PNoteRepository(_context);

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


