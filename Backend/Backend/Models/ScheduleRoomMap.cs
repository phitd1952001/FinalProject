using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class ScheduleRoomMap
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SchedulerId { get; set; }
    
    [Required]
    public int RoomId { get; set; }
    
    [Required]
    public string StudentIds { get; set; }
}