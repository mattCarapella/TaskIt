#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Repositories;
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
        public async Task<IActionResult> Index()
        {
            var taskManagerContext = _context.Tickets.Include(t => t.Project);
            return View(await taskManagerContext.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }


        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description");
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
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == HttpContext.User.Identity.Name);
                    ticket.TicketId = Guid.NewGuid();
                    ticket.SubmittedBy = user;
                    var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == ticket.ProjectId);
                    project.Tickets.Add(ticket);
                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
            return View(ticket);
        }


        // POST: Tickets/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Guid? id)
        {
            if (id != null)
            {
                return NotFound();
            }

            var ticketToUpdate = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == id);

            if (await TryUpdateModelAsync<Ticket>(
                    ticketToUpdate,
                    "",
                    t => t.Title, t => t.Description, t => t.TicketType, t => t.Status,
                    t => t.Priority, t => t.Tag, t => t.GoalDate, t => t.ProjectId, t => t.UpdatedAt))
            {
                try
                {
                    ticketToUpdate.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "There was a problem updating this ticket. Please try again.");
                }
            }
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
            //var ticket = await _unitOfWork.Ticket.GetTicket(id);
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