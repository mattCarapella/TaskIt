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
        [Display(Name = "Submitted for Review")]
        SUBMITTED = 2,
        [Display(Name = "Completed")]
        COMPLETED = 3
    }

    public enum Priority
    {
        [Display(Name = "1 (Lowest Priority)")]
        ONE,
        [Display(Name = "2")]
        TWO,
        [Display(Name = "3")]
        THREE,
        [Display(Name = "4")]
        FOUR,
        [Display(Name = "5 (Highest Priority)")]
        FIVE
    }

}
