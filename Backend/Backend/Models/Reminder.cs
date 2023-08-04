using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Reminder
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Message { get; set; }
    [Required]
    public string To { get; set; }
    [Required]
    public DateTime DateTimePerformed { get; set; }
}