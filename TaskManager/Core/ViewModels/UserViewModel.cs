using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using TaskManager.Areas.Identity.Data;
using TaskManager.Models;

namespace TaskManager.Core.ViewModels;

public class UserViewModel
{
    public ApplicationUser User { get; set; }

    public List<string>? Roles { get; set; }

    public ICollection<ProjectAssignment>? ProjectAssignments { get; set; }

    public ICollection<TicketAssignment>? TicketsAssignments { get; set;  }

}
