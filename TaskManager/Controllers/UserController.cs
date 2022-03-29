using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.Areas.Identity.Data;
using TaskManager.Core.Repositories;
using TaskManager.Core.ViewModels;

namespace TaskManager.Controllers;

public class UserController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UserController(IUnitOfWork unitOfWork, SignInManager<ApplicationUser> signInManager)
    {
        _unitOfWork = unitOfWork;
        _signInManager = signInManager;
    }

    public IActionResult Index()
    {
        var users = _unitOfWork.User.GetUsers();
        return View(users);
    }


    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _unitOfWork.User.GetUserWithProjects(id);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _signInManager.UserManager.GetRolesAsync(user);
        var vm = new UserViewModel
        {
            User = user,
            Roles = (List<string>)userRoles
        };

        return View(vm);
    }


    public async Task<IActionResult> Edit(string id )
    {
        var user = _unitOfWork.User.GetUser(id);
        var roles = _unitOfWork.Role.GetRoles();

        var userRoles = await _signInManager.UserManager.GetRolesAsync(user);

        var roleItems = new List<SelectListItem>();

        //foreach (var role in roles)
        //{
        //    var hasRole = selectedUserRoles.Any(r => r.Contains(role.Name));
        //    roleItems.Add(new SelectListItem(role.Name, role.Id, hasRole));
        //}

        var roleItemsSelect = roles.Select(role =>
            // SelectListItem(Text, Value, Selected)
            new SelectListItem(
                role.Name, 
                role.Id, 
                userRoles.Any(r => r.Contains(role.Name)))).ToList();

        var vm = new EditUserViewModel
        {
            User = user,
            //Roles = roleItems
            Roles = roleItemsSelect
        };

        return View(vm);
    }


    [HttpPost]
    public async Task<IActionResult> OnPostAsync(EditUserViewModel data)
    {
        var user = _unitOfWork.User.GetUser(data.User.Id);
        if (user == null)
        {
            return NotFound();
        }
        var userRolesInDb = await _signInManager.UserManager.GetRolesAsync(user);

        var rolesToAdd = new List<string>();
        var rolesToDelete = new List<string>();

        foreach (var role in data.Roles)
        {
            // Check if role is assigned to user in db
            var assignedInDb = userRolesInDb.FirstOrDefault(r => r == role.Text);

            if (role.Selected)
            {
                if (assignedInDb == null)
                {
                    rolesToAdd.Add(role.Text);
                }
            }
            else
            {
                if (assignedInDb != null)
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

        user.FirstName = data.User.FirstName;
        user.LastName = data.User.LastName;
        user.UserName = data.User.UserName;
        user.Email = data.User.Email;
        user.JobTitle = data.User.JobTitle;
        user.EmployeeID = data.User.EmployeeID;
        user.Email = data.User.Email;

        _unitOfWork.User.UpdateUser(user);

        return RedirectToAction("Details", new { id = user.Id });

    }

}
