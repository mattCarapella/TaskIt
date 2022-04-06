using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TaskManager.Models;

namespace TaskManager.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Required, StringLength(50), Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required, StringLength(50), Display(Name = "Last Name")]
    public string LastName { get; set; }

    public string? ProfilePicture { get; set; }

    [Display(Name = "Employee ID")]
    public string? EmployeeID { get; set; }

    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

    [Display(Name = "Name")]
    public string FullName
    {
        get
        {
            return FirstName + " " + LastName;
        }
    }

    [Display(Name = "Created On"), DataType(DataType.Date)]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Last Login"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:g}")]
    public DateTime? LastLoggedInAt  { get; set; }




    public ICollection<ProjectAssignment> Projects { get; set; } = new List<ProjectAssignment>();

    public ICollection<TicketAssignment> Tickets { get; set; } = new List<TicketAssignment>();

    public ICollection<PNote> Notes { get; set; } = new List<PNote>();
}

public class ApplicationRole : IdentityRole
{
}