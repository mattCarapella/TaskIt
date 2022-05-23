using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class FileModel
{
    public Guid Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; }

    [Required]
    public byte[] Data { get; set; } = Array.Empty<byte>();

    [Required, StringLength(50)]
    public string FileType { get; set; }

    [Required, StringLength(10)]
    public string Extension { get; set; }

    [Required, StringLength(300)]
    public string Description { get; set; }

    public string UploadedByUserId { get; set; } = String.Empty;

    public Guid TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
}
