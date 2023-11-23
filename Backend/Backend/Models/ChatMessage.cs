using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class ChatMessage
{
    public ChatMessage()
    {
        CreateAt = DateTime.UtcNow;
    }
    
    [Key]
    public int Id { get; set; }
    [Required]
    public string Type { get; set; }
    [Required]
    public string Message { get; set; }
    [Required]
    public int ChatId { get; set; }
    [ForeignKey("ChatId")]
    public Chat Chat { get; set; }
    [Required]
    public int FromUserId { get; set; }

    public DateTime CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}