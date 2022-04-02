using TaskManager.Models;

namespace TaskManager.Core.ViewModels.Note
{
    public class PNoteCreateViewModel
    {
        public PNote Note { get; set; }
        public Models.Project? Project { get; set; }

        public Guid ProjectId { get; set; }
    }
}
