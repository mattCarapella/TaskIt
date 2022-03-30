using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.Models;

namespace TaskManager.Core.ViewModels;

public class AddUserProjectViewModel
{
    public Project Project { get; set; }

    public string UserId { get; set; }
    public List<SelectListItem> ListOfUsers { get; set; }
}
