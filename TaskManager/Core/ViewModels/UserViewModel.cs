using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using TaskManager.Areas.Identity.Data;

namespace TaskManager.Core.ViewModels;

public class UserViewModel
{
    public ApplicationUser User { get; set; }

    public List<string>? Roles { get; set; }


}
