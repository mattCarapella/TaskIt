using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Authorization;
using TaskManager.Core.Repositories;
using TaskManager.Core.ViewModels.Note;
using TaskManager.Data;
using TaskManager.Models;
using Constants = TaskManager.Core.Constants;

namespace TaskManager.Controllers
{
    public class TNotesController : Controller
    {
        private readonly TaskManagerContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public TNotesController(TaskManagerContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }


        // GET: TNotes/Details/5
        [Authorize(Roles = $"{Constants.Roles.Administrator}")]
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var tNote = await _unitOfWork.TNoteRepository.GetNote(id);
            if (tNote is null) return NotFound();

            return View(tNote);
        }



        // GET: TNotes/Create
        public IActionResult Create(Guid ticketid)
        {
            TNoteCreateViewModel vm = new TNoteCreateViewModel
            {
                TicketId = ticketid
            };
            return View(vm);
        }

        // POST: TNotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title, Content, Link, ApplicationUserId, TicketId")] TNote note)
        {
            if (ModelState.IsValid)
            {
                note.Id = Guid.NewGuid();
                var ticket = await _unitOfWork.TicketRepository.GetTicket(note.TicketId);
                if (ticket is null) return NotFound();
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.GetUserId());
                ticket.TNotes.Add(note);
                note.ApplicationUser = currentUser;
                await _unitOfWork.TNoteRepository.AddNote(note);
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Details", "Tickets", new { id = ticket.TicketId });
            }
            TNoteCreateViewModel vm = new TNoteCreateViewModel
            {
                TicketId = note.TicketId
            };
            return View(vm);
        }



        // GET: TNotes/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var tNote = await _unitOfWork.TNoteRepository.GetNote(id);
            if (tNote is null) return NotFound();

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, tNote, TaskOperations.Update);
            if (!isAuthorized.Succeeded) return Forbid();

            return View(tNote);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var noteToUpdate = await _unitOfWork.TNoteRepository.GetNote(id);
            if (noteToUpdate is null) return NotFound();

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, noteToUpdate, TaskOperations.Update);
            if (!isAuthorized.Succeeded) return Forbid();

            if (await TryUpdateModelAsync<TNote>(
                    noteToUpdate,
                    "",
                    n => n.Title, n => n.Content, n => n.Link!))
            {
                try
                {
                    noteToUpdate.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.SaveAsync();
                    return RedirectToAction("Details", "Tickets", new { id = noteToUpdate.TicketId });

                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "There was a problem updating this note. Please try again.");
                }
            }
            return View(noteToUpdate);
        }



        // GET: TNotes/Delete/5
        [Authorize(Roles = $"{Constants.Roles.Administrator}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            //var tNote = await _context.TNote
            //    .Include(t => t.ApplicationUser)
            //    .Include(t => t.Ticket)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            var tNote = await _unitOfWork.TNoteRepository.GetNote(id);
            if (tNote == null) return NotFound();

            return View(tNote);
        }

        // POST: Projects/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var note = await _unitOfWork.TNoteRepository.GetNote(id);
            if (note is null)
            {
                return NotFound();
            }

            try
            {
                await _unitOfWork.TNoteRepository.DeleteNote(note.Id);
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Details", "Tickets", new { id = note.TicketId });
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }


    }
}
