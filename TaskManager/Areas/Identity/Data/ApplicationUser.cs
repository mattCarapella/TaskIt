using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TaskManager.Models;

namespace TaskManager.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Photo { get; set; } = "";

    public string? EmployeeID { get; set; }

    public string? JobTitle { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLoggedInAt  { get; set; }



    public ICollection<ProjectAssignment>? Projects { get; set; }

}

public class ApplicationRole : IdentityRole
{
}