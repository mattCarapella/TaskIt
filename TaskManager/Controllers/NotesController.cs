#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.ViewModels.Note;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class NotesController : Controller
    {
        private readonly TaskManagerContext _context;

        public NotesController(TaskManagerContext context)
        {
            _context = context;
        }

        // GET: Notes
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Notes.ToListAsync());
        //}

        //// GET: Notes/Details/5
        //public async Task<IActionResult> Details(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var note = await _context.Notes
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (note == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(note);
        //}

        //// GET: Notes/Create
        //public async Task<IActionResult> Create(Guid? projectId)
        //{
        //    var pID = projectId;

        //    PNoteCreateViewModel vm = new PNoteCreateViewModel
        //    {
        //        Project = await _context.Projects.FindAsync(pID)
        //    };

        //    return View(vm);
        //}

        //// POST: Notes/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Title,Content,Link,ProjectId")] Note note)
        //{

        //    var projectId = note.ProjectId;
            
        //    if (ModelState.IsValid)
        //    {
        //        note.Id = Guid.NewGuid();
        //        _context.Add(note);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(note);
        //}

        //// GET: Notes/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var note = await _context.Notes.FindAsync(id);
        //    if (note == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(note);
        //}

        //// POST: Notes/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("Title,Content,Link,UpdatedAt")] Note note)
        //{
        //    if (id != note.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(note);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!NoteExists(note.Id))
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
        //    return View(note);
        //}

        //// GET: Notes/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var note = await _context.Notes
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (note == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(note);
        //}

        //// POST: Notes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var note = await _context.Notes.FindAsync(id);
        //    _context.Notes.Remove(note);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool NoteExists(Guid id)
        //{
        //    return _context.Notes.Any(e => e.Id == id);
        //}
    }
}
