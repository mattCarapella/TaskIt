using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.Identity;
using TaskManager.Core.ViewModels.Note;
using TaskManager.Data;
using TaskManager.Models;
using Microsoft.AspNetCore.Authorization;
using Constants = TaskManager.Core.Constants;
using TaskManager.Core.Repositories;

namespace TaskManager.Controllers
{
    
    public class PNotesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PNotesController(TaskManagerContext context, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: PNotes/Create
        public async Task<IActionResult> Create(Guid projectid)
        {
            var project = await _unitOfWork.ProjectRepository.GetProjectWithUsers(projectid);
            var currentUser = await _unitOfWork.UserRepository.GetCurrentUser();
            if (project is null)
            {
                return NotFound();
            }

            if (project.Contributers!.Any(u => u.ApplicationUserId == currentUser!.Id))
            {
                PNoteCreateViewModel vm = new PNoteCreateViewModel
                {
                    ProjectId = projectid
                };
                return View(vm);
            }

            return RedirectToAction("Details", "Projects", new { id = projectid });

        }


        // POST: PNotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,Link,ProjectId,ApplicationUser,ApplicationUserId")] PNote note)
        {
            if (ModelState.IsValid)
            {
                note.Id = Guid.NewGuid();
                var project = await _unitOfWork.ProjectRepository.GetProject(note.ProjectId);
                var currentUser = await _unitOfWork.UserRepository.GetCurrentUser();
                project.Notes.Add(note);
                note.ApplicationUser = currentUser;
                await _unitOfWork.PNoteRepository.AddNote(note);
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Details", "Projects", new { id = project.Id });
            }
            PNoteCreateViewModel vm = new PNoteCreateViewModel
            {
                ProjectId = note.ProjectId
            };
            return View(vm);
        }


        // GET: PNotes/Edit/{id}
        [Authorize(Roles = $"{Constants.Roles.Administrator}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var pNote = await _unitOfWork.PNoteRepository.GetNote(id);
            if (pNote is null)
            {
                return NotFound();
            }
            return View(pNote);
        }


        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var noteToUpdate = await _unitOfWork.PNoteRepository.GetNote(id);
            if (await TryUpdateModelAsync<PNote>(
                    noteToUpdate,
                    "",
                    n => n.Title, n => n.Content, n => n.Link))
            {
                try
                {
                    noteToUpdate.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.SaveAsync();
                    return RedirectToAction("Details", "Projects", new { id = noteToUpdate.ProjectId });

                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "There was a problem updating this note. Please try again.");
                }
            }
            return View(noteToUpdate);
        }

        
        [Authorize(Roles = $"{Constants.Roles.Administrator}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var pNote = await _unitOfWork.PNoteRepository.GetNote(id);
            if (pNote is null)
            {
                return NotFound();
            }

            return View(pNote);
        }

        // POST: Projects/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = Constants.Policies.RequireAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var note = await _unitOfWork.PNoteRepository.GetNote(id);
            if (note is null)
            {
                return NotFound();
            }

            try
            {
                await _unitOfWork.PNoteRepository.DeleteNote(note.Id);
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Details", "Projects", new { id = note.ProjectId });
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }


    }
}






// GET: PNotes
//[Authorize(Roles = $"{Constants.Roles.Administrator}")]
//public async Task<IActionResult> Index()
//{
//    var taskManagerContext = _context.PNote.Include(p => p.Project);
//    return View(await taskManagerContext.ToListAsync());
//}


//// GET: PNotes/Details/5
//[Authorize(Roles = $"{Constants.Roles.Administrator}")]
//public async Task<IActionResult> Details(Guid? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }

//    var pNote = await _context.PNote
//        .Include(p => p.Project)
//        .AsNoTracking()
//        .FirstOrDefaultAsync(m => m.Id == id);
//    if (pNote == null)
//    {
//        return NotFound();
//    }

//    return View(pNote);
//}