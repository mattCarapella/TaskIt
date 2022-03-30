using TaskManager.Areas.Identity.Data;

namespace TaskManager.Models
{
    public class TicketAssignment
    {
        public Guid TicketAssignmentId { get; set; }
        public Guid TicketId { get; set; }
        public int ApplicationUserId { get; set; }

        public Ticket Ticket { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
