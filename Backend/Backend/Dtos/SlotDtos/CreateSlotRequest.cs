using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.SlotDtos;

public class CreateSlotRequest
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public int Duration { get; set; }
    
    [Required]
    public int SubjectId { get; set; }
    
    [Required]
    public int RoomId { get; set; }
}