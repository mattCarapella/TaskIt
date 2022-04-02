#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core;
using TaskManager.Core.Repositories;
using TaskManager.Core.ViewModels;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TicketsController : Controller
    {
        private readonly TaskManagerContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public TicketsController(TaskManagerContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        // GET: Tickets
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ProjectNameSortParam"] = sortOrder == "projectName" ? "projectName_desc" : "projectName";
            ViewData["TicketTitleSortParam"] = sortOrder == "ticketTitle" ? "ticketTitle_desc" : "ticketTitle";
            ViewData["GoalDateSortParam"] = sortOrder == "goalDate" ? "goalDate_desc" : "goalDate";
            ViewData["StatusSortParam"] = sortOrder == "status" ? "status_desc" : "status";
            ViewData["PrioritySortParam"] = sortOrder == "priority" ? "priority_desc" : "priority";
            ViewData["CreatedOnSortParam"] = sortOrder == "createdOn" ? "createdOn_desc" : "createdOn";
            ViewData["UpvotesSortParam"] = sortOrder == "upvotes" ? "upvotes_desc" : "upvotes";
            ViewData["TicketTypeSortParam"] = sortOrder == "ticketType" ? "ticketType_desc" : "ticketType";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var ticketContext = _context.Tickets.Include(t => t.Project);
            var tickets = from t in ticketContext select t;
            
            if (!String.IsNullOrEmpty(searchString))
            {
                tickets = tickets.Where(t => t.Title.Contains(searchString) || t.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "projectName":
                    tickets = tickets.OrderBy(t => t.Project.Name);
                    break;
                case "projectName_desc":
                    tickets = tickets.OrderByDescending(t => t.Project.Name);
                    break;
                case "ticketTitle":
                    tickets = tickets.OrderBy(t => t.Title);
                    break;
                case "ticketTitle_desc":
                    tickets = tickets.OrderByDescending(t => t.Title);
                    break;
                case "goalDate":
                    tickets = tickets.OrderBy(t => t.GoalDate);
                    break;
                case "goalDate_desc":
                    tickets = tickets.OrderByDescending(t => t.GoalDate);
                    break;
                case "status":
                    tickets = tickets.OrderBy(t => t.Status);
                    break;
                case "status_desc":
                    tickets = tickets.OrderByDescending(t => t.Status);
                    break;
                case "priority":
                    tickets = tickets.OrderBy(t => t.Priority);
                    break;
                case "priority_desc":
                    tickets = tickets.OrderByDescending(t => t.Priority);
                    break;
                case "createdOn":
                    tickets = tickets.OrderBy(t => t.CreatedAt);
                    break;
                case "createdOn_desc":
                    tickets = tickets.OrderByDescending(t => t.CreatedAt);
                    break;
                case "upvotes":
                    tickets = tickets.OrderBy(t => t.Upvotes);
                    break;
                case "upvotes_desc":
                    tickets = tickets.OrderByDescending(t => t.Upvotes);
                    break;
                case "ticketType":
                    tickets = tickets.OrderBy(t => t.TicketType);
                    break;
                case "ticketType_desc":
                    tickets = tickets.OrderByDescending(t => t.TicketType);
                    break;
                default:
                    tickets = tickets.OrderBy(t => t.Project.Name);
                    break;
            }

            int pageSize = 9;
            return View(await PaginatedList<Ticket>.CreateAsync(tickets.AsNoTracking(), pageNumber ?? 1, pageSize));
            //return View(await tickets.AsNoTracking().ToListAsync());
        }


        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Project)
                .Include(t => t.SubmittedBy)
                .Include(c => c.AssignedTo)
                    .ThenInclude(u => u.ApplicationUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }



            return View(ticket);
        }

        public bool IsUserAssigned(string userId, Guid ticketId)
        {
            var ticketAssignment = _context.TicketAssignments.FirstOrDefault(t => t.TicketId == ticketId);
            if (ticketAssignment.ApplicationUserId == userId)
            {
                return true;
            }
            return false;
        }


        // GET: Tickets/Create
        public async Task<IActionResult> Create(string? projectId)
        {
            if (!String.IsNullOrEmpty(projectId))
            {
                var id = Guid.Parse(projectId);
                var projectAssign = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
                IEnumerable<Project> pl = new List<Project>() { projectAssign };
                ViewData["Project"] = new SelectList(pl, "Id", "Description");
            }
            else
            {
                ViewData["Project"] = new SelectList(_context.Projects, "Id", "Description");
            }
            return View();
        }


        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Tag,TicketType,Status,Priority," +
            "GoalDate,ProjectId,SubmittedBy")] Ticket ticket)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.GetUserId());
                    ticket.TicketId = Guid.NewGuid();
                    ticket.SubmittedBy = currentUser;
                    var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == ticket.ProjectId);
                    project.Tickets.Add(ticket);
                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["Project"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
                return View(ticket);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save project.");
            }

            return View(ticket);
        }


        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["Project"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
            //ViewData["Project"] = new SelectList(_context.Projects, "Id", "Description", ticket.Project.Id); ********
            return View(ticket);
        }


        // POST: Tickets/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketToUpdate = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == id);

            if (await TryUpdateModelAsync<Ticket>(
                    ticketToUpdate,
                    "",
                    t => t.Title, t => t.Description, t => t.TicketType, t => t.Status,
                    t => t.Priority, t => t.Tag, t => t.GoalDate, t => t.ProjectId))
            {
                try
                {
                    ticketToUpdate.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = ticketToUpdate.TicketId });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "There was a problem updating this ticket. Please try again.");
                }
            }
            ViewData["Project"] = new SelectList(_context.Projects, "Id", "Description", ticketToUpdate.ProjectId);
            return View(ticketToUpdate);
        }


        // GET: Tickets/Delete/{id}
        public async Task<IActionResult> Delete(Guid? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "There was a problem deleting this ticket. Please try again.";
            }

            return View(ticket);
        }


        // POST: Tickets/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid? id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }


        // GET: AssignUser/{id}
        public async Task<IActionResult> AssignUser(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(c => c.AssignedTo)
                .ThenInclude(u => u.ApplicationUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            var vm = GetViewModel(ticket);
            return View(vm);
        }


        // POST: AssignUser/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUser(Guid? id, AssignUserTicketViewModel ticketViewModel)
        {
            var selectedUserId = ticketViewModel.UserId;
            if (selectedUserId == null || id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FirstOrDefaultAsync(p => p.TicketId == id);
            var selectedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == selectedUserId);
            var assignee = new TicketAssignment
            {
                TicketAssignmentId = Guid.NewGuid(),
                ApplicationUser = selectedUser,
                ApplicationUserId = selectedUserId,
                Ticket = ticket,
                TicketId = ticket.TicketId,
                AssignedByUsedId = User.Identity.GetUserId()
            };

            try
            {

                _context.TicketAssignments.Add(assignee);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = ticket.TicketId });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to add user.");
            }

            var vm = GetViewModel(ticket);
            return View(vm);
        }


        private AssignUserTicketViewModel GetViewModel(Ticket ticket)
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
            var vm = new AssignUserTicketViewModel
            {
                Ticket = ticket,
                ListOfUsers = userList
            };
            return vm;
        }


        private bool TicketExists(Guid id)
        {
            return _context.Tickets.Any(e => e.TicketId == id);
        }
    }
}













//// POST: Tickets/Edit/{id}
//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> Edit(Guid id, [Bind("TicketId,Title,Description,Tag,TicketType,Status,Upvotes," +
//    "Priority,GoalDate,CreatedAt,UpdatedAt,ProjectId")] Ticket ticket)
//{
//    if (id != ticket.TicketId)
//    {
//        return NotFound();
//    }

//    if (ModelState.IsValid)
//    {
//        try
//        {
//            _context.Update(ticket);
//            await _context.SaveChangesAsync();
//        }
//        catch (DbUpdateConcurrencyException)
//        {
//            if (!TicketExists(ticket.TicketId))
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
//    ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
//    return View(ticket);
//}




//// GET: Tickets/Delete/{id}
//public async Task<IActionResult> Delete(Guid? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }

//    var ticket = await _context.Tickets
//        .Include(t => t.Project)
//        .FirstOrDefaultAsync(m => m.TicketId == id);
//    if (ticket == null)
//    {
//        return NotFound();
//    }

//    return View(ticket);
//}

//// POST: Tickets/Delete/5
//[HttpPost, ActionName("Delete")]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> DeleteConfirmed(Guid id)
//{
//    var ticket = await _context.Tickets.FindAsync(id);
//    _context.Tickets.Remove(ticket);
//    await _context.SaveChangesAsync();
//    return RedirectToAction(nameof(Index));
//}






//// POST: Tickets/Edit/{id}
//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> Edit(Guid id, [Bind("Title,Description,Tag,TicketType,Status,Upvotes," +
//    "Priority,GoalDate")] Ticket ticket)
//{
//    if (id != ticket.TicketId)
//    {
//        return NotFound();
//    }

//    if (ModelState.IsValid)
//    {
//        try
//        {
//            _context.Update(ticket);
//            await _context.SaveChangesAsync();
//        }
//        catch (DbUpdateConcurrencyException)
//        {
//            if (!TicketExists(ticket.TicketId))
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
//    //ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
//    return View(ticket);
//}