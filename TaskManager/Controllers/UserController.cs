using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Areas.Identity.Data;
using TaskManager.Core.Repositories;
using TaskManager.Core.ViewModels;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers;

public class UserController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly TaskManagerContext _context;

    public UserController(IUnitOfWork unitOfWork, SignInManager<ApplicationUser> signInManager,
        IWebHostEnvironment webHostEnvironment, TaskManagerContext context)
    {
        _unitOfWork = unitOfWork;
        _signInManager = signInManager;
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }

    public IActionResult Index()
    {
        var users = _unitOfWork.UserRepository.GetUsers();
        return View(users);
    }


    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user =  _unitOfWork.UserRepository.GetUser(id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.Tickets.Any())
        {
            var tickets = user.Tickets.Where(s => s.Ticket.Status != Core.Enums.Enums.Status.COMPLETED);
        }


        var pa = _context.ProjectAssignments
            .Include(p => p.Project)
            .ThenInclude(c => c.Contributers)
            .Where(u => u.ApplicationUserId == id)
            .ToList();

        var ta = await _context.TicketAssignments
            .Include(t => t.Ticket)
            .ThenInclude(a => a.AssignedTo)
            
            .Where(u => u.ApplicationUserId == id)
            .ToListAsync();

        var open = ta.Where(x => x.Ticket.Status != Core.Enums.Enums.Status.COMPLETED);

        var userRoles = await _signInManager.UserManager.GetRolesAsync(user);
        var vm = new UserViewModel
        {
            User = user,
            Roles = (List<string>)userRoles,
            ProjectAssignments = pa,
            TicketsAssignments = ta
        };

        

        return View(vm);
    }


    public async Task<IActionResult> Edit(string id )
    {
        var user = _unitOfWork.UserRepository.GetUser(id);
        var roles = _unitOfWork.RoleRepository.GetRoles();

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
        var user = _unitOfWork.UserRepository.GetUser(data.User.Id);
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

        string uploadedFileName = UploadFile(data) ?? user.ProfilePicture;

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
        string fileName = null;
        if (data.ProfilePicture != null)
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
