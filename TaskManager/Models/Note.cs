using TaskManager.Areas.Identity.Data;

namespace TaskManager.Models;

public class Note
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public string? Link { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

}
