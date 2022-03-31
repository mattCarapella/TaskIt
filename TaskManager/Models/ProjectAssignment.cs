using TaskManager.Areas.Identity.Data;

namespace TaskManager.Models;

public class ProjectAssignment
{
    public Guid ProjectAssignmentId { get; set; }

    public string? ApplicationUserId { get; set; }
    public Guid? ProjectId { get; set; }
    public bool IsManager { get; set; }
    public ApplicationUser? AssignedBy { get; set; }


    public Project? Project { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
}
