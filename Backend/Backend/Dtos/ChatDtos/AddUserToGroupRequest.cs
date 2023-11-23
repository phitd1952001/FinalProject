namespace Backend.Dtos.ChatDtos;

public class AddUserToGroupRequest
{
    public int ChatId { get; set; }
    public int UserId { get; set; }
}