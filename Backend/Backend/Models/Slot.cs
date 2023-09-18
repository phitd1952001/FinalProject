using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class Slot
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public int Duration { get; set; }
    
    [Required]
    public int SubjectId { get; set; }
    [ForeignKey("SubjectId")]
    public Subject Subject { get; set; }
    
    [Required]
    public int RoomId { get; set; }
    [ForeignKey("RoomId")]
    public Room Room { get; set; }
}