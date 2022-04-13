using TaskManager.Models;

namespace TaskManager.Core.Repositories;

public interface IPNoteRepository
{
    Task<PNote> GetNote(Guid noteId, bool tracking = true);
    Task AddNote(PNote note);
    Task DeleteNote(Guid noteId);
    bool PNoteExists(Guid id);
}
