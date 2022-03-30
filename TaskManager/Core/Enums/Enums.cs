using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.Enums;

public class Enums
{
    public enum TicketType
    {
        [Display(Name = "Bug")]
        BUG = 0,
        [Display(Name = "Feature")]
        FEATURE = 1,
    }

    public enum Status
    {
        [Display(Name = "To Do")]
        TODO,
        [Display(Name = "In Progress")]
        INPROGRESS = 1,
        [Display(Name = "Submitted")]
        SUBMITTED = 2,
        [Display(Name = "Completed")]
        COMPLETED = 3
    }

}
