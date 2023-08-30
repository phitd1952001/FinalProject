using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.RoomDtos;

public class UpdateRoomRequest
{
    [Required] 
    public string Name { get; set; }
 
    [Required] 
    public string NumberOfSeat { get; set; }
}