﻿using TaskManager.Models;
using TaskManager.Repositories;

namespace TaskManager.Core.Repositories;

public interface ITicketRepository
{
   

    Task<Ticket> GetTicket(Guid ticketId, bool tracking = true);
    Task<Ticket> GetTicketWithProject(Guid ticketId);
    Task<Ticket> GetTicketWithAssignedUsers(Guid ticketId);
    Task<Ticket> GetTicketWithProjectAndUserDetails(Guid ticketId);
    //Task<ICollection<Ticket>> GetTicketWithProjectAndUserDetails(Guid ticketId);
    Task<ICollection<Ticket>> GetTickets();
    Task<List<Ticket>> GetTicketsWithProjects();
    Task<List<Ticket>> GetClosedTicketsWithProjects();
    Task<List<Ticket>> GetTicketsToAssign();
    Task<List<Ticket>> GetTicketsForReview();
    Task<List<Ticket>> GetTicketsAssignedToUser(string userId);
    Task AddTicket(Ticket ticket);
    Task DeleteTicket(Guid ticketId);

}
