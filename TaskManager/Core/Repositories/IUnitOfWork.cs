namespace TaskManager.Core.Repositories;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    IProjectRepository ProjectRepository { get;  }
    ITicketRepository TicketRepository { get; }
    IProjectAssignmentRepository ProjectAssignmentRepository { get; }
    ITicketAssignmentRepository TicketAssignmentRepository { get; }
    ITNoteRepository TNoteRepository { get; }
    IPNoteRepository PNoteRepository { get; }
    Task<bool> SaveAsync();
}





//IUserRepository User { get; }
//IRoleRepository Role { get; }