using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.SlotDtos;

public class UpdateSlotRequest
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