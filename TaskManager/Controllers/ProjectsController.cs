#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core;
using TaskManager.Core.Enums;
using TaskManager.Core.Repositories;
using TaskManager.Core.ViewModels;
using TaskManager.Core.ViewModels.Project;
using TaskManager.Data;
using TaskManager.Models;
using Constants = TaskManager.Core.Constants;

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

            //var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.GetUserId());

            var projectList = await _unitOfWork.ProjectAssignmentRepository.GetProjectAssignmentsWithTicketsForUser(User.Identity.GetUserId());
            var projects = from p in projectList select p.Project;

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

            var pList = projects.ToList();
            int pageSize = 10;
            return View(PaginatedList<Project>.Create(pList, pageNumber ?? 1, pageSize));       //   pList should have .AsNoTracking()... find out if it can work with async calls
            //return View(await projects.AsNoTracking().ToListAsync());
        }

        // GET: AllProjects
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        public async Task<IActionResult> AllProjects(string sortOrder, string searchString, string currentFilter, int? pageNumber)
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

            var projectList = await _unitOfWork.ProjectRepository.GetProjectsWithTickets();
            var projects = from p in projectList select p;
            //projects = projects.AsQueryable();

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

            var pList = projects.ToList();
            int pageSize = 10;
            return View(PaginatedList<Project>.Create(pList, pageNumber ?? 1, pageSize));       //   pList should have .AsNoTracking()... find out if it can work with async calls
            //return View(await projects.AsNoTracking().ToListAsync());
        }


        // GET: Projects/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var project = await _unitOfWork.ProjectRepository.GetProjectWithTicketsNotesUsers(id);

            if (project == null)
            {
                return NotFound();
            }

            var contributers = project.Contributers.ToList();
            
            var openTickets = project.Tickets.Where(t => t.Status != Enums.Status.COMPLETED).ToList();
            var closedTickets = project.Tickets.Where(t => t.Status == Enums.Status.COMPLETED).ToList();
            var notes = project.Notes.ToList();

            var ticketList = await _unitOfWork.TicketAssignmentRepository.GetTicketAssignmentsWithProjectForUser(User.Identity.GetUserId());

            

            var vm = new ProjectDetailsViewModel()
            {
                Project = project,
                Contributers = contributers,    
                OpenTickets = openTickets,
                ClosedTickets = closedTickets,
                Notes = notes
            };

            return View(vm);
        }


        // GET: Projects/Create
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Projects/Create
        [HttpPost]
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Tag,GoalDate")] Project project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.GetUserId());
                    var contributer = new ProjectAssignment
                    {
                        ProjectAssignmentId = Guid.NewGuid(),
                        ApplicationUser = currentUser,
                        Project = project,
                        IsManager = true,
                        AssignedByUsedId = currentUser.Id
                    };

                    project.Id = Guid.NewGuid();
                    project.Contributers.Add(contributer);
                    project.CreatedByUserId = currentUser.Id;
                    await _unitOfWork.ProjectRepository.AddProject(project);
                    await _unitOfWork.SaveAsync();
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
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var project = await _unitOfWork.ProjectRepository.GetProject(id);

            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }


        // POST: Projects/Edit/{id}
        [HttpPost, ActionName("Edit")]
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var projectToUpdate = await _unitOfWork.ProjectRepository.GetProject(id, true);

            if (await TryUpdateModelAsync<Project>(
                projectToUpdate, "", p => p.Name, p => p.Description, p => p.Tag, p => p.GoalDate))
            {
                try
                {
                    projectToUpdate.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.SaveAsync();
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
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        public async Task<IActionResult> Delete(Guid id, bool? saveChangeErrors = false)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var project = await _unitOfWork.ProjectRepository.GetProject(id, false);

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
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var project = await _unitOfWork.ProjectRepository.GetProject(id);
            if (project == null)
            {
                return NotFound();
            }

            try
            {
                await _unitOfWork.ProjectRepository.DeleteProject(project.Id);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }


        // GET: ManageUsers/{id}
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> ManageUsers(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var project = await _unitOfWork.ProjectRepository.GetProjectWithUsers(id);

            if (project == null)
            {
                return NotFound();
            }

            var vm = GetViewModel(project);
            return View(vm);
        }


        // POST: ManageUsers/{id}
        [HttpPost]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUsers(Guid id, AddUserProjectViewModel projectViewModel)
        {
            var selectedUserId = projectViewModel.UserId;
            if (selectedUserId == null || id == Guid.Empty)
            {
                return NotFound();
            }

            var project = await _unitOfWork.ProjectRepository.GetProject(id);
            var selectedUser = await _unitOfWork.UserRepository.GetUserAsync(selectedUserId);
            var contributer = new ProjectAssignment
            {
                ProjectAssignmentId = Guid.NewGuid(),
                ApplicationUser = selectedUser,
                ApplicationUserId = selectedUserId,
                Project = project,
                ProjectId = project.Id,
                IsManager = false,
                AssignedByUsedId = User.Identity.GetUserId()
            };

            try
            {
                await _unitOfWork.ProjectAssignmentRepository.AddProjectAssignment(contributer); 
                await _unitOfWork.SaveAsync();
                return RedirectToAction("ManageUsers", "Projects", project);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to add user.");
            }

            var vm = GetViewModel(project);
            return View(vm);
        }


        // POST: RemoveUser
        [HttpPost]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> RemoveUser(Guid projectId, Guid paId)
        {
            if (paId == Guid.Empty || projectId == Guid.Empty)
            {
                return NotFound();
            }

            await _unitOfWork.ProjectAssignmentRepository.DeleteProjectAssignment(paId);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(ManageUsers), new { id = projectId });
        }





        private AddUserProjectViewModel GetViewModel(Project project)
        {
            /* Creates a dropdown list of users to add to a project. Users who are already contributers
               are removed from the dropdown here as well. */

            var inList = _context.ProjectAssignments
                                .Where(p => p.ProjectId == project.Id)
                                .AsNoTracking()
                                .Include(a => a.ApplicationUser);

            var notInList = _context.Users
                                .Where(a => !inList.Any(b => b.ApplicationUserId == a.Id))
                                .AsNoTracking()
                                .ToList();

            var userList = (from user in notInList
                            select new SelectListItem()
                            {
                                Text = user.FullName,
                                Value = user.Id.ToString()
                            }).ToList();

            userList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = String.Empty
            });

            ViewBag.ListOfUsers = userList.OrderBy(x => x.Text);
            var vm = new AddUserProjectViewModel
            {
                Project = project,
                ListOfUsers = userList
            };

            return vm;
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










//public async Task<IActionResult> Details(Guid id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }
//     ....
            //var project = await _context.Projects
            //    .Include(t => t.Tickets)
            //    .Include(n => n.Notes)
            //    .ThenInclude(u => u.ApplicationUser)
            //    .Include(c => c.Contributers)
            //    .ThenInclude(u => u.ApplicationUser)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(x => x.Id == id);