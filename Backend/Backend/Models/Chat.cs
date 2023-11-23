using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Chat
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Type { get; set; }
}