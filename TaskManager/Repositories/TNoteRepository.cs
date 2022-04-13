using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Repositories;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Repositories;

public class TNoteRepository : ITNoteRepository
{
    private readonly TaskManagerContext _context;

    public TNoteRepository(TaskManagerContext context)
    {
        _context = context;
    }

    public async Task<TNote> GetNote(Guid noteId, bool tracking = true)
    {
        if (!tracking)
        {
            return await _context.TNote
                .AsNoTracking()
                .Include(t => t.Ticket)
                .FirstOrDefaultAsync(n => n.Id == noteId);
        }
        return await _context.TNote.FirstOrDefaultAsync(n => n.Id == noteId);
    }
    public async Task AddNote(TNote note)
    {
        await _context.TNote.AddAsync(note);
    }

    public async Task DeleteNote(Guid noteId)
    {
        var note = await _context.TNote.FirstOrDefaultAsync(n => n.Id == noteId);
        _context.TNote.Remove(note);
    }

}
