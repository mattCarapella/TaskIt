using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.Areas.Identity.Data;

namespace TaskManager.Core.ViewModels;

public class EditUserViewModel
{
    public ApplicationUser User { get; set; }

    public IFormFile? ProfilePicture { get; set; }

    public IList<SelectListItem> Roles { get; set; }

}
