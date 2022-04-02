using TaskManager.Areas.Identity.Data;

namespace TaskManager.Models;

public class Note
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public string? Link { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }


    public ApplicationUser ApplicationUser { get; set; }


    public Project? Project { get; set; }

    public Guid? ProjectId { get; set; }

}
