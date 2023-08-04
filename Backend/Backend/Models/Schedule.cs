using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class Schedule
{
    [Key]
    public int Id { get; set; }
    
    public DateTime DateTime { get; set; }
    public TimeSpan Duration { get; set; }
    
    [Required]
    public int RoomId { get; set; }
    [ForeignKey("RoomId")]
    public Checkin Checkin { get; set; }
    
    [Required]
    public int UserId { get; set; }

    [Required]
    public int SubjectId { get; set; }
    
}