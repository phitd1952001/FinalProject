namespace Backend.Dtos.SlotDtos;

public class ExcelMappingModel
{
    public string Name { get; set; }
    
    public string StartTime { get; set; } // MM/dd/yyyy-22:30
    
    public int Duration { get; set; }
  
    public string SubjectCode { get; set; }
    
    public string RoomName { get; set; }
}