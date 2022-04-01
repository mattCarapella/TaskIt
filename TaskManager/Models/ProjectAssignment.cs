using TaskManager.Areas.Identity.Data;

namespace TaskManager.Models;

public class ProjectAssignment
{
    public Guid ProjectAssignmentId { get; set; }

    public string? ApplicationUserId { get; set; }
    public Guid? ProjectId { get; set; }
    

    public virtual Project? Project { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; }


    public bool IsManager { get; set; }
    public string? AssignedByUsedId { get; set; }

}
