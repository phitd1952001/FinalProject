using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class Message
{
    [Key]
    public int Id { get; set; }

    public string Text { get; set; }
    
    public DateTime DateTime { get; set; }
    
    [Required]
    public string WithUserId { get; set; }
    
    [Required]
    public int UserId { get; set; }
}