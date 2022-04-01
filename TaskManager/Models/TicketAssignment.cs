using TaskManager.Areas.Identity.Data;

namespace TaskManager.Models
{
    public class TicketAssignment
    {
        public Guid TicketAssignmentId { get; set; }
        public Guid? TicketId { get; set; }
        public string? ApplicationUserId { get; set; }

        public virtual Ticket? Ticket { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }

        //public ApplicationUser? AssignedBy { get; set; }
    }
}
