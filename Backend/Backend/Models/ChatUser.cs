using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class ChatUser
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public Account Account { get; set; }
    [Required]
    public int ChatId { get; set; }
    [ForeignKey("ChatId")]
    public Chat Chat { get; set; }
}