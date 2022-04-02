#nullable disable
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


namespace TaskManager.Controllers
{
    public class PNotesController : Controller
    {
        private readonly TaskManagerContext _context;

        public PNotesController(TaskManagerContext context)
        {
            _context = context;
        }


        // GET: PNotes
        public async Task<IActionResult> Index()
        {
            var taskManagerContext = _context.PNote.Include(p => p.Project);
            return View(await taskManagerContext.ToListAsync());
        }


        // GET: PNotes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pNote = await _context.PNote
                .Include(p => p.Project)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pNote == null)
            {
                return NotFound();
            }

            return View(pNote);
        }


        // GET: PNotes/Create
        public async Task<IActionResult> Create(Guid projectid)
        {
            PNoteCreateViewModel vm = new PNoteCreateViewModel
            {
                ProjectId = projectid
            };
            return View(vm);
        }


        // POST: PNotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,Link,ProjectId")] PNote note)
        {
            var n = note;
            if (ModelState.IsValid)
            {
                note.Id = Guid.NewGuid();
                var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == note.ProjectId);
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.GetUserId());
                project.Notes.Add(note);
                note.ApplicationUser = currentUser;
                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PNoteCreateViewModel vm = new PNoteCreateViewModel
            {
                ProjectId = note.ProjectId
            };
            return View(vm);
        }


        // GET: PNotes/Edit/{id}
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pNote = await _context.PNote.FindAsync(id);
            if (pNote == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", pNote.ProjectId);
            return View(pNote);
        }


        // POST: PNotes/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Content,Link,CreatedAt,UpdatedAt,ProjectId")] PNote pNote)
        {
            if (id != pNote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pNote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PNoteExists(pNote.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", pNote.ProjectId);
            return View(pNote);
        }


        // GET: PNotes/Delete/{id}
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pNote = await _context.PNote
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pNote == null)
            {
                return NotFound();
            }

            return View(pNote);
        }


        // POST: PNotes/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var pNote = await _context.PNote.FindAsync(id);
            _context.PNote.Remove(pNote);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool PNoteExists(Guid id)
        {
            return _context.PNote.Any(e => e.Id == id);
        }
    }
}
