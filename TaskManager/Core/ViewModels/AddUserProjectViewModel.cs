using Microsoft.AspNetCore.Mvc.Rendering;

namespace TaskManager.Core.ViewModels;

public class AddUserProjectViewModel
{
    public Models.Project Project { get; set; }

    public string UserId { get; set; }
    public List<SelectListItem> ListOfUsers { get; set; }
}
