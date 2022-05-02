#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Areas.Identity.Data;
using TaskManager.Core;
using TaskManager.Core.Repositories;
using TaskManager.Core.ViewModels;
using TaskManager.Core.ViewModels.Ticket;
using TaskManager.Data;
using TaskManager.Models;
using HtmlAgilityPack;
using Constants = TaskManager.Core.Constants;

namespace TaskManager.Controllers
{
    public class TicketsController : Controller
    {
        private readonly TaskManagerContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public TicketsController(TaskManagerContext context, SignInManager<ApplicationUser> signInManager, IUnitOfWork unitOfWork)
        {
            _context = context;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        /*******************************************************************************************************************/
        /*  USER INDEX PAGES */
        /*******************************************************************************************************************/

        // GET: Tickets
        /* Index of current user's assigned open tickets for all projects */
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

            if (searchString is not null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            //var ticketContext = _context.Tickets.Include(t => t.Project);   
            //var ticketList = await _unitOfWork.TicketRepository.GetTicketsWithProjects();
            //var tickets = from t in ticketList select t;

            var ticketList = await _unitOfWork.TicketAssignmentRepository.GetTicketAssignmentsWithProjectForUser(User.Identity.GetUserId());
            var tickets = from t in ticketList select t.Ticket;


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
                    tickets = tickets.OrderBy(t => t.Status);
                    break;
            }
            var tList = tickets.ToList();
            int pageSize = 9;

            // Temporarily sets AsNoTracking() which is needed in return
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var paginatedTicketList = PaginatedList<Ticket>.Create(tList, pageNumber ?? 1, pageSize);
            return View(paginatedTicketList);
            //return View(await tickets.AsNoTracking().ToListAsync());
        }



        // GET: Tickets/Closed
        /* Index of the current user's closed tickets for all projects */
        public async Task<IActionResult> Closed(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ProjectNameSortParam"] = sortOrder == "projectName" ? "projectName_desc" : "projectName";
            ViewData["TicketTitleSortParam"] = sortOrder == "ticketTitle" ? "ticketTitle_desc" : "ticketTitle";
            ViewData["GoalDateSortParam"] = sortOrder == "goalDate" ? "goalDate_desc" : "goalDate";
            ViewData["StatusSortParam"] = sortOrder == "status" ? "status_desc" : "status";
            ViewData["PrioritySortParam"] = sortOrder == "priority" ? "priority_desc" : "priority";
            ViewData["CreatedAtSortParam"] = sortOrder == "createdAt" ? "createdAt_desc" : "createdAt";
            ViewData["ClosedAtSortParam"] = sortOrder == "closedAt" ? "closedAt_desc" : "closedAt";
            ViewData["TicketTypeSortParam"] = sortOrder == "ticketType" ? "ticketType_desc" : "ticketType";
            ViewData["CurrentFilter"] = searchString;

            if (searchString is not null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var ticketList = await _unitOfWork.TicketAssignmentRepository.GetClosedTicketAssignmentsWithProjectForUser(User.Identity.GetUserId());
            var tickets = from t in ticketList select t.Ticket;

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
                case "createdAt":
                    tickets = tickets.OrderBy(t => t.CreatedAt);
                    break;
                case "createdAt_desc":
                    tickets = tickets.OrderByDescending(t => t.CreatedAt);
                    break;
                case "closedAt":
                    tickets = tickets.OrderBy(t => t.ClosedAt);
                    break;
                case "closedAt_desc":
                    tickets = tickets.OrderByDescending(t => t.ClosedAt);
                    break;
                case "ticketType":
                    tickets = tickets.OrderBy(t => t.TicketType);
                    break;
                case "ticketType_desc":
                    tickets = tickets.OrderByDescending(t => t.TicketType);
                    break;
                default:
                    tickets = tickets.OrderByDescending(t => t.ClosedAt);
                    break;
            }
            var tList = tickets.ToList();
            int pageSize = 9;

            // Temporarily sets AsNoTracking() which is needed in return
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var paginatedTicketList = PaginatedList<Ticket>.Create(tList, pageNumber ?? 1, pageSize);
            return View(paginatedTicketList);
            //return View(await tickets.AsNoTracking().ToListAsync());
        }



        /*******************************************************************************************************************/
        /* ADMIN & MANAGER INDEX PAGES */
        /*******************************************************************************************************************/

        // GET: Tickets/AllTickets
        /* Index of all open tickets for all projects */
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        public async Task<IActionResult> AllTickets(string sortOrder, string searchString, string currentFilter, int? pageNumber)
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

            if (searchString is not null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var ticketList = await _unitOfWork.TicketRepository.GetTicketsWithProjects();
            var tickets = from t in ticketList select t;

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
            var tList = tickets.ToList();
            int pageSize = 9;

            // Temporarily sets AsNoTracking() which is needed in return
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            return View(PaginatedList<Ticket>.Create(tList, pageNumber ?? 1, pageSize));

            //return View(await PaginatedListAsync2<Ticket>.CreateAsync(tickets, pageNumber ?? 1, pageSize));
            //return View(await tickets.AsNoTracking().ToListAsync());
        }



        // GET: Tickets/ToAssign
        /* Index of all recently created tickets that have not yet been assigned to a user */ 
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        public async Task<IActionResult> ToAssign(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ProjectNameSortParam"] = sortOrder == "projectName" ? "projectName_desc" : "projectName";
            ViewData["TicketTitleSortParam"] = sortOrder == "ticketTitle" ? "ticketTitle_desc" : "ticketTitle";
            ViewData["GoalDateSortParam"] = sortOrder == "goalDate" ? "goalDate_desc" : "goalDate";
            ViewData["PrioritySortParam"] = sortOrder == "priority" ? "priority_desc" : "priority";
            ViewData["CreatedOnSortParam"] = sortOrder == "createdOn" ? "createdOn_desc" : "createdOn";
            ViewData["TicketTypeSortParam"] = sortOrder == "ticketType" ? "ticketType_desc" : "ticketType";
            ViewData["CurrentFilter"] = searchString;

            if (searchString is not null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var ticketList = await _unitOfWork.TicketRepository.GetTicketsToAssign();
            var tickets = from t in ticketList select t;

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
            var tList = tickets.ToList();
            int pageSize = 9;

            // Temporarily sets AsNoTracking() which is needed in return
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            return View(PaginatedList<Ticket>.Create(tList, pageNumber ?? 1, pageSize));
        }



        // GET: Tickets/AllClosed
        /* Index of all closed tickets */
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        public async Task<IActionResult> AllClosed(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ProjectNameSortParam"] = sortOrder == "projectName" ? "projectName_desc" : "projectName";
            ViewData["TicketTitleSortParam"] = sortOrder == "ticketTitle" ? "ticketTitle_desc" : "ticketTitle";
            ViewData["GoalDateSortParam"] = sortOrder == "goalDate" ? "goalDate_desc" : "goalDate";
            ViewData["PrioritySortParam"] = sortOrder == "priority" ? "priority_desc" : "priority";
            ViewData["CreatedAtSortParam"] = sortOrder == "createdAt" ? "createdAt_desc" : "createdAt";
            ViewData["ClosedAtSortParam"] = sortOrder == "closedAt" ? "closedAt_desc" : "closedAt";
            ViewData["TicketTypeSortParam"] = sortOrder == "ticketType" ? "ticketType_desc" : "ticketType";
            ViewData["CurrentFilter"] = searchString;

            if (searchString is not null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var ticketList = await _unitOfWork.TicketRepository.GetClosedTicketsWithProjects();
            var tickets = from t in ticketList select t;

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
                case "priority":
                    tickets = tickets.OrderBy(t => t.Priority);
                    break;
                case "priority_desc":
                    tickets = tickets.OrderByDescending(t => t.Priority);
                    break;
                case "createdAt":
                    tickets = tickets.OrderBy(t => t.CreatedAt);
                    break;
                case "createdAt_desc":
                    tickets = tickets.OrderByDescending(t => t.CreatedAt);
                    break;
                case "closedAt":
                    tickets = tickets.OrderBy(t => t.ClosedAt);
                    break;
                case "closedAt_desc":
                    tickets = tickets.OrderByDescending(t => t.ClosedAt);
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
            var tList = tickets.ToList();
            int pageSize = 9;

            // Temporarily sets AsNoTracking() which is needed in return
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            return View(PaginatedList<Ticket>.Create(tList, pageNumber ?? 1, pageSize));
        }



        // GET: Tickets/SubmittedForReview
        /* Index of all open tickets submitted for review by a user */
        //[Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        [Authorize(Policy = Constants.Policies.ElevatedRights)]
        public async Task<IActionResult> SubmittedForReview(string sortOrder, string searchString, string currentFilter, int? pageNumber)
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

            if (searchString is not null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var ticketList = await _unitOfWork.TicketRepository.GetTicketsForReview();
            var tickets = from t in ticketList select t;

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
            var tList = tickets.ToList();
            int pageSize = 9;

            // Temporarily sets AsNoTracking() which is needed in return
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            return View(PaginatedList<Ticket>.Create(tList, pageNumber ?? 1, pageSize));
        }



        /*******************************************************************************************************************/
        /* TICKET CRUD */
        /*******************************************************************************************************************/

        // GET: Tickets/Details/{Id}
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var ticket = await _unitOfWork.TicketRepository.GetTicketWithProjectAndUserDetails(id);
            if (ticket is null)
            {
                return NotFound();
            }

            var vm = new TicketDetailsViewModel()
            {
                Ticket = ticket,
                AssignedTo = ticket.AssignedTo.ToList(),
                Project = ticket.Project,
                SubmittedBy = ticket.SubmittedBy,
                Notes = ticket.TNotes
            };

            return View(vm);
        }



        // GET: Tickets/Create
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> Create(string? projectId)
        {
            if (!String.IsNullOrEmpty(projectId))
            {
                var projectAssign = await _unitOfWork.ProjectRepository.GetProject(Guid.Parse(projectId));
                IEnumerable<Project> pl = new List<Project>() { projectAssign };
                ViewData["Project"] = new SelectList(pl, "Id", "Name");
            }
            else
            {
                ViewData["Project"] = new SelectList(_context.Projects, "Id", "Name");
            }
            return View();
        }

        // POST: Tickets/Create
        [HttpPost]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DescriptionNoHtml,Tag,TicketType,Status,Priority," +
            "GoalDate,ProjectId,SubmittedBy")] Ticket ticket)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.GetUserId());
                    ticket.TicketId = Guid.NewGuid();
                    ticket.SubmittedBy = currentUser;
                    HtmlDocument doc = new HtmlDocument();
                    doc.OptionFixNestedTags = false;
                    doc.LoadHtml(ticket.Description);
                    ticket.DescriptionNoHtml = doc.DocumentNode.InnerText;
                    var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == ticket.ProjectId);
                    project.Tickets.Add(ticket);
                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = ticket.TicketId });
                }
                ViewData["Project"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
                return View(ticket);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save project.");
            }

            return View(ticket);
        }



        // GET: Tickets/Edit/{Id}
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var ticket = await _unitOfWork.TicketRepository.GetTicket(id);
            if (ticket is null)
            {
                return NotFound();
            }
            ViewData["Project"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            return View(ticket);
        }

        // POST: Tickets/Edit/{Id}
        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var ticketToUpdate = await _unitOfWork.TicketRepository.GetTicket(id);
            HtmlDocument doc = new HtmlDocument();
            doc.OptionFixNestedTags = false;
            doc.LoadHtml(ticketToUpdate.Description);
            var noHtml = doc.DocumentNode.InnerText;

            if (await TryUpdateModelAsync<Ticket>(
                    ticketToUpdate,
                    "",
                    t => t.Title, t => t.Description, t => t.DescriptionNoHtml, t => t.TicketType, t => t.Status,
                    t => t.Priority, t => t.Tag, t => t.GoalDate, t => t.ProjectId))
            {
                try
                {
                    if (ticketToUpdate.Description != noHtml)
                    {
                        ticketToUpdate.DescriptionNoHtml = noHtml;
                    }
                    ticketToUpdate.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.SaveAsync();
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



        // GET: Tickets/Delete/{Id}
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        public async Task<IActionResult> Delete(Guid id, bool? saveChangesError = false)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var ticket = await _unitOfWork.TicketRepository.GetTicket(id, false);
            if (ticket is null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "There was a problem deleting this ticket. Please try again.";
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/{Id}
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (id == Guid.Empty)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _unitOfWork.TicketRepository.DeleteTicket(id);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }



        /*******************************************************************************************************************/
        /* USER ASSIGNMENT */
        /*******************************************************************************************************************/

        // GET: ManageUsers/{Id}
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> ManageUsers(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var ticket = await _unitOfWork.TicketRepository.GetTicketWithProjectAndUserDetails(id);
            if (ticket is null)
            {
                return NotFound();
            }

            var vm = GetViewModel(ticket);
            return View(vm);
        }



        // POST: ManageUsers/{Id}
        [HttpPost]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUsers(Guid id, AssignUserTicketViewModel ticketViewModel)
        {
            var selectedUserId = ticketViewModel.UserId;
            if (selectedUserId is null || id == Guid.Empty)
            {
                return NotFound();
            }


            var ticket = await _unitOfWork.TicketRepository.GetTicket(id);
            var selectedUser = await _unitOfWork.UserRepository.GetUserAsync(selectedUserId);
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
                await _unitOfWork.TicketAssignmentRepository.AddTicketAssignment(assignee);
                if (ticket.Status == Core.Enums.Enums.Status.TODO)
                {
                    ticket.Status = Core.Enums.Enums.Status.INPROGRESS;
                }
                await _unitOfWork.SaveAsync();
                return RedirectToAction("ManageUsers", new { id = ticket.TicketId });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to add user.");
            }

            var vm = GetViewModel(ticket);
            return View(vm);
        }



        // POST: RemoveUser
        /* Unassigns ticket for a user */
        [HttpPost]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> RemoveUser(Guid ticketId, Guid taId)
        {
            if (taId == Guid.Empty || ticketId == Guid.Empty)
            {
                return NotFound();
            }

            await _unitOfWork.TicketAssignmentRepository.DeleteTicketAssignment(taId);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(ManageUsers), new { id = ticketId });
        }



        /* Creates a dropdown list of users to add to a ticket. Users who are already contributers
              are removed from the dropdown here as well. */
        private AssignUserTicketViewModel GetViewModel(Ticket ticket)
        {
            var inList = _context.TicketAssignments
                                .Where(p => p.TicketId == ticket.TicketId)
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

            ViewBag.ListOfUsers = userList;
            var vm = new AssignUserTicketViewModel
            {
                Ticket = ticket,
                Project = ticket.Project,
                ListOfUsers = userList
            };
            return vm;
        }




        /*******************************************************************************************************************/
        /* TICKET SUBMISSION */
        /*******************************************************************************************************************/


        // POST: SubmitForReview
        /* Puts ticket in admin's list of completed tickets to review */
        [HttpPost]
        public async Task<IActionResult> SubmitForReview(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                return NotFound();
            }

            var ticketToSubmit = await _unitOfWork.TicketRepository.GetTicket(ticketId);

            try
            {
                ticketToSubmit.Status = Core.Enums.Enums.Status.SUBMITTED;
                ticketToSubmit.UpdatedAt = DateTime.Now;
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "There was a problem submitting this ticket. Please try again.");
            }
            return RedirectToAction("Details", new { id = ticketToSubmit.TicketId });
        }



        // POST: CancelSubmission
        /* Change ticket status from COMPLETED back to INPROGRESS, removes it from admin's tickets to review, and puts it back in the list of open tickets */
        [HttpPost]
        public async Task<IActionResult> CancelSubmission(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                return NotFound();
            }

            var ticketToCancel = await _unitOfWork.TicketRepository.GetTicket(ticketId);

            try
            {
                ticketToCancel.Status = Core.Enums.Enums.Status.INPROGRESS;
                ticketToCancel.UpdatedAt = DateTime.Now;
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "There was a problem cancelling ticket submission. Please try again.");
            }
            return RedirectToAction("Details", new { id = ticketToCancel.TicketId });
        }



        // POST: MarkAsCompleted
        // Change Ticket status to Completed and add it to closed ticket
        [HttpPost]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public async Task<IActionResult> MarkAsCompleted(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                return NotFound();
            }

            var ticketToSubmit = await _unitOfWork.TicketRepository.GetTicket(ticketId);

            try
            {
                ticketToSubmit.Status = Core.Enums.Enums.Status.COMPLETED;
                ticketToSubmit.ClosedAt = DateTime.Now;
                ticketToSubmit.UpdatedAt = DateTime.Now;
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "There was a problem closing this ticket. Please try again.");
            }
            return RedirectToAction("Details", new { id = ticketToSubmit.TicketId });
        }



        private bool TicketExists(Guid id)
        {
            return _context.Tickets.Any(e => e.TicketId == id);
        }
    }
}
















//private AssignUserTicketViewModel GetViewModel(Ticket ticket)
//{
//    var userList = (from user in _context.Users.OrderBy(u => u.LastName)
//                    select new SelectListItem()
//                    {
//                        Text = user.FullName, // + " (" + user.EmployeeID + ")",
//                        Value = user.Id.ToString()
//                    }).ToList();

//    userList.Insert(0, new SelectListItem()
//    {
//        Text = "----Select----",
//        Value = String.Empty
//    });

//    ViewBag.ListOfUsers = userList;
//    var vm = new AssignUserTicketViewModel
//    {
//        Ticket = ticket,
//        Project = ticket.Project,
//        ListOfUsers = userList
//    };
//    return vm;
//}




//// POST: RemoveUser/{id}
//[HttpPost]
//public async Task<IActionResult> RemoveUser(string id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }
//    var entryToRemove = await _context.TicketAssignments.FirstOrDefaultAsync(t => t.ApplicationUserId == id);  // ***************** ProjectAssignmentRepository
//    var ticketId = entryToRemove.TicketId;
//    _context.TicketAssignments.Remove(entryToRemove);          // ***************** ProjectAssignmentRepository
//    await _unitOfWork.SaveAsync();
//    return RedirectToAction(nameof(ManageUsers), new { id = ticketId });
//}





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