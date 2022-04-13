using TaskManager.Models;

namespace TaskManager.Core.ViewModels.Note
{
    public class TNoteCreateViewModel
    {
        public TNote Note { get; set; }
        public Models.Ticket? Ticket { get; set; }

        public Guid TicketId { get; set; }
    }
}
