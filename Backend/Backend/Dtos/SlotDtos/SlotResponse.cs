namespace Backend.Dtos.SlotDtos;

public class SlotResponse
{
    public int SlotId { get; set; }
    
    public string Name { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public int Duration { get; set; }
    
    public int SubjectId { get; set; }
  
    public string SubjectName { get; set; }
    
    public int RoomId { get; set; }
    
    public string RoomName { get; set; }
}