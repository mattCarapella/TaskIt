using TaskManager.Core.Repositories;
using TaskManager.Data;

namespace TaskManager.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly TaskManagerContext _context;

    public UnitOfWork(TaskManagerContext context)
    {
        _context = context;
    }

    public IUserRepository UserRepository => new UserRepository(_context);
    public IRoleRepository RoleRepository => new RoleRepository(_context);
    public IProjectRepository ProjectRepository => new ProjectRepository(_context);
    public ITicketRepository TicketRepository => new TicketRepository(_context);

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


