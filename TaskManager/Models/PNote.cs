using System.ComponentModel.DataAnnotations;
using TaskManager.Areas.Identity.Data;

namespace TaskManager.Models;

public class PNote
{
    public Guid Id { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; }

    [Required, StringLength(2000)]
    public string Content { get; set; }

    public string? Link { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public string ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }

}
