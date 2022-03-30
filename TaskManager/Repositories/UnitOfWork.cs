using TaskManager.Core.Repositories;

namespace TaskManager.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public IUserRepository User { get;  }
    public IRoleRepository Role { get; }

    //public ITicketRepository Ticket { get; }

    public UnitOfWork(IUserRepository user, IRoleRepository role)//, ITicketRepository ticket)
    {
        User = user;
        Role = role;
        //Ticket = ticket;
    }

}
