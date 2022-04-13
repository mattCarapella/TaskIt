using TaskManager.Models;

namespace TaskManager.Core.Repositories;

public interface ITNoteRepository
{
    Task<TNote> GetNote(Guid noteId, bool tracking = true);
    Task AddNote(TNote note);
    Task DeleteNote(Guid noteId);
}
