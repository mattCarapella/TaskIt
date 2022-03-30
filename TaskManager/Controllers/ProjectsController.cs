#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core;
using TaskManager.Core.Enums;
using TaskManager.Core.Repositories;
using TaskManager.Core.ViewModels;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly TaskManagerContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectsController(TaskManagerContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        // GET: Projects
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CreatedOnSortParam"] = sortOrder == "createdOn" ? "createdOn_desc" : "createdOn";
            ViewData["NameSortParam"] = sortOrder == "name" ? "name_desc" : "name";
            ViewData["GoalDateSortParam"] = sortOrder == "goalDate" ? "goalDate_desc" : "goalDate";
            ViewData["OpenTicketSortParam"] = sortOrder == "openTickets" ? "openTickets_desc" : "openTickets";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var projectsInContext = _context.Projects.Include(p => p.Tickets);

            var projects = from p in projectsInContext select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(s => s.Name.Contains(searchString) || s.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name":
                    projects = projects.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    projects = projects.OrderByDescending(p => p.Name);
                    break;
                case "createdOn_desc":
                    projects = projects.OrderByDescending(p => p.CreatedAt);
                    break;
                case "goalDate":
                    projects = projects.OrderBy(p => p.GoalDate);
                    break;
                case "goalDate_desc":
                    projects = projects.OrderByDescending(p => p.GoalDate);
                    break;
                case "openTickets":
                    projects = projects.OrderBy(p => p.Tickets.Where(t => t.Status != Enums.Status.COMPLETED).Count());
                    break;
                case "openTickets_desc":
                    projects = projects.OrderByDescending(p => p.Tickets.Where(t => t.Status != Enums.Status.COMPLETED).Count());
                    break;
                default:
                    projects = projects.OrderBy(p => p.CreatedAt);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Project>.CreateAsync(projects.AsNoTracking(), pageNumber ?? 1, pageSize));
            //return View(await projects.AsNoTracking().ToListAsync());


            // ::: TO DO:::
            //
            // If user is admin or project manager => get all projects
            // If user role is developer => get assigned projects
        }

        // GET: Projects/Details/{id}
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(t => t.Tickets)
                .Include(c => c.Contributers)
                .ThenInclude(u => u.ApplicationUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }


        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Tag,GoalDate")] Project project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == HttpContext.User.Identity.Name);

                    var contributer = new ProjectAssignment
                    {
                        ProjectAssignmentId = Guid.NewGuid(),
                        ApplicationUser = user,
                        Project = project,
                        IsManager = true
                    };

                    project.Id = Guid.NewGuid();
                    project.Contributers.Add(contributer);
                    _context.Projects.Add(project);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save project.");
            }
            
            return View(project);
        }


        // GET: Projects/Edit/{id}
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }


        // POST: Projects/Edit/{id}
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var projectToUpdate = await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);
            if (await TryUpdateModelAsync<Project>(
                projectToUpdate, "", p => p.Name, p => p.Description, p => p.Tag, p => p.GoalDate))
            {
                try
                {
                    projectToUpdate.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to update project.");
                }
            }
            return View(projectToUpdate);
        }


        // GET: Projects/Delete/{id}
        public async Task<IActionResult> Delete(Guid? id, bool? saveChangeErrors = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            if (saveChangeErrors.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Project deletion failed.";
            }

            return View(project);

        }


        // POST: Projects/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            try
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }


        // GET: AddUser/{id}
        public async Task<IActionResult> AddUser(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(c => c.Contributers)
                .ThenInclude(u => u.ApplicationUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            var vm = GetViewModel(project);
            return View(vm);
        }


        // POST: AddUser/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(Guid? id, AddUserProjectViewModel projectViewModel)
        {
            var selectedUserId = projectViewModel.UserId;
            if (selectedUserId == null || id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
            var selectedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == selectedUserId);
            var contributer = new ProjectAssignment
            {
                ProjectAssignmentId = Guid.NewGuid(),
                ApplicationUser = selectedUser,
                ApplicationUserId = selectedUserId,
                Project = project,
                ProjectId = project.Id,
                IsManager = false
            };

            try
            {
                _context.ProjectAssignments.Add(contributer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Projects", project);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to add user.");
            }

            var vm = GetViewModel(project);
            return View(vm);
        }


        private AddUserProjectViewModel GetViewModel(Project project)
        {
            var userList = (from user in _context.Users
                            select new SelectListItem()
                            {
                                Text = user.UserName,
                                Value = user.Id.ToString()
                            }).ToList();

            userList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = String.Empty
            });

            ViewBag.ListOfUsers = userList;
            var vm = new AddUserProjectViewModel
            {
                Project = project,
                ListOfUsers = userList
            };
            return vm;
        }


        // POST: RemoveUser
        [HttpPost]
        public async Task<IActionResult> RemoveUser(Guid paID)
        {
            var entryToRemove = await _context.ProjectAssignments.FirstOrDefaultAsync(p => p.ProjectAssignmentId == paID);
            _context.ProjectAssignments.Remove(entryToRemove);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ProjectExists(Guid id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }

    }
}











//var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == id);


// To protect from overposting attacks, enable the specific properties you want to bind to.
// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.



//public async Task<IActionResult> Delete(Guid? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }

//    var project = await _context.Projects
//        .FirstOrDefaultAsync(m => m.Id == id);
//    if (project == null)
//    {
//        return NotFound();
//    }

//    return View(project);
//}

// POST: Projects/Delete/5
//[HttpPost, ActionName("Delete")]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> DeleteConfirmed(Guid id)
//{
//    var project = await _context.Projects.FindAsync(id);
//    _context.Projects.Remove(project);
//    await _context.SaveChangesAsync();
//    return RedirectToAction(nameof(Index));
//}



// POST: Projects/Edit/5
// To protect from overposting attacks, enable the specific properties you want to bind to.
// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Description,Tag,GoalDate,CreatedAt,UpdatedAt")] Project project)
//{
//    if (id != project.Id)
//    {
//        return NotFound();
//    }

//    if (ModelState.IsValid)
//    {
//        try
//        {
//            _context.Update(project);
//            await _context.SaveChangesAsync();
//        }
//        catch (DbUpdateConcurrencyException)
//        {
//            if (!ProjectExists(project.Id))
//            {
//                return NotFound();
//            }
//            else
//            {
//                throw;
//            }
//        }
//        return RedirectToAction(nameof(Index));
//    }
//    return View(project);
//}