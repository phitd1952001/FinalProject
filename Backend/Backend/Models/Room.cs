using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Room
{
    [Key] 
    public int Id { get; set; }
    
    [Required] 
    public string Name { get; set; }
    
    [Required] 
    public int NumberOfSeat { get; set; }
}