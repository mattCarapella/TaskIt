using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.Areas.Identity.Data;
using TaskManager.Core;
using TaskManager.Core.Repositories;
using TaskManager.Core.ViewModels;

namespace TaskManager.Controllers;

public class UserController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UserController(IUnitOfWork unitOfWork, SignInManager<ApplicationUser> signInManager,
        IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _signInManager = signInManager;
        _webHostEnvironment = webHostEnvironment;
    }


    public IActionResult Index(string sortOrder, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["CreatedAtSortParam"] = sortOrder == "createdAt" ? "createdAt_desc" : "createdAt";
        ViewData["LastNameSortParam"] = sortOrder == "lastName" ? "lastName_desc" : "lastName";
        ViewData["JobTitleSortParam"] = sortOrder == "jobTitle" ? "jobTitle_desc" : "jobTitle";

        var userList = _unitOfWork.UserRepository.GetUsers().ToList();
        var users = from u in userList select u;

        switch (sortOrder)
        {
            case "lastName_desc":
                users = users.OrderByDescending(p => p.LastName);
                break;
            case "createdAt":
                users = users.OrderBy(u => u.CreatedAt);
                break;
            case "createdAt_desc":
                users = users.OrderByDescending(u => u.CreatedAt);
                break;
            case "jobTitle":
                users = users.OrderBy(u => u.JobTitle);
                break;
            case "jobTitle_desc":
                users = users.OrderBy(u => u.JobTitle);
                break;
            default:
                users = users.OrderBy(p => p.LastName);
                break;
        }

        var uList = users.ToList();
        int pageSize = 10;
        return View(PaginatedList<ApplicationUser>.Create(uList, pageNumber ?? 1, pageSize));
    }



    public async Task<IActionResult> Details(string id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var user = await _unitOfWork.UserRepository.GetUserWithProjectsAndTickets(id);
        if (user is null)
        {
            return NotFound();
        }

        var open = user.Tickets.Where(x => x.Ticket!.Status != Core.Enums.Enums.Status.COMPLETED);
        var userRoles = await _signInManager.UserManager.GetRolesAsync(user);
        var vm = new UserViewModel
        {
            User = user,
            Roles = (List<string>)userRoles,
            ProjectAssignments = user.Projects,
            TicketsAssignments = user.Tickets
        };

        return View(vm);
    }


    public async Task<IActionResult> Edit(string id )
    {
        if (id is null)
        {
            return NotFound();
        }

        var user = await _unitOfWork.UserRepository.GetUserAsync(id);
        var roles = _unitOfWork.RoleRepository.GetRoles();
        var userRoles = await _signInManager.UserManager.GetRolesAsync(user);
        var roleItems = new List<SelectListItem>();

        var roleItemsSelect = roles.Select(role =>
            new SelectListItem(
                role.Name, 
                role.Id, 
                userRoles.Any(r => r.Contains(role.Name)))).ToList();

        var vm = new EditUserViewModel
        {
            User = user,
            Roles = roleItemsSelect
        };

        return View(vm);
    }


    [HttpPost]
    public async Task<IActionResult> OnPostAsync(EditUserViewModel data)
    {
        var user = await _unitOfWork.UserRepository.GetUserAsync(data.User.Id);
        if (user is null)
        {
            return NotFound();
        }
        var userRolesInDb = await _signInManager.UserManager.GetRolesAsync(user);
        var rolesToAdd = new List<string>();
        var rolesToDelete = new List<string>();

        foreach (var role in data.Roles)
        {
            var assignedInDb = userRolesInDb.FirstOrDefault(r => r == role.Text);
            if (role.Selected)
            {
                if (assignedInDb is null)
                {
                    rolesToAdd.Add(role.Text);
                }
            }
            else
            {
                if (assignedInDb is not null)
                {
                    rolesToDelete.Add(role.Text);
                }
            }
        }
        if (rolesToAdd.Any())
        {
            await _signInManager.UserManager.AddToRolesAsync(user, rolesToAdd);
        }
        if (rolesToDelete.Any())
        {
            await _signInManager.UserManager.RemoveFromRolesAsync(user, rolesToDelete);
        }
        string uploadedFileName = UploadFile(data) ?? user.ProfilePicture!;
        user.FirstName = data.User.FirstName;
        user.LastName = data.User.LastName;
        user.UserName = data.User.UserName;
        user.Email = data.User.Email;
        user.JobTitle = data.User.JobTitle;
        user.EmployeeID = data.User.EmployeeID;
        user.Email = data.User.Email;
        user.ProfilePicture = uploadedFileName;

        _unitOfWork.UserRepository.UpdateUser(user);
        await _unitOfWork.SaveAsync();
        return RedirectToAction("Details", new { id = user.Id });
    }

    private string UploadFile(EditUserViewModel data)
    {
        string fileName = "";
        if (data.ProfilePicture is not null)
        {
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            fileName = Guid.NewGuid().ToString() + "_" + data.ProfilePicture.FileName;
            string filePath = Path.Combine(uploadDir, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                data.ProfilePicture.CopyTo(fileStream);
            }
        }
        return fileName;
    }
}
