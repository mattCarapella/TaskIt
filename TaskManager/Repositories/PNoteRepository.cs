using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Repositories;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Repositories;

public class PNoteRepository : IPNoteRepository
{
    private readonly TaskManagerContext _context;

    public PNoteRepository(TaskManagerContext context)
    {
        _context = context;
    }

    public async Task<PNote> GetNote(Guid noteId, bool tracking = true)
    {
        if (!tracking)
        {
            return await _context.PNote
                .AsNoTracking()
                .Include(p => p.Project)
                .FirstOrDefaultAsync(n => n.Id == noteId);
        }
        return await _context.PNote.FirstOrDefaultAsync(n => n.Id == noteId);
    }
    public async Task AddNote(PNote note)
    {
        await _context.PNote.AddAsync(note);
    }

    public async Task DeleteNote(Guid noteId)
    {
        var note = await _context.PNote.FirstOrDefaultAsync(n => n.Id == noteId);
        _context.PNote.Remove(note);
    }

    public bool PNoteExists(Guid id)
    {
        return _context.PNote.Any(e => e.Id == id);
    }
}
