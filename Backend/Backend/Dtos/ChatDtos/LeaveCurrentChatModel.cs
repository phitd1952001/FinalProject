namespace Backend.Dtos.ChatDtos;

public class LeaveCurrentChatModel
{
    public int ChatId { get; set; }
    public int UserId { get; set; }
    public int CurrentUserId { get; set; }
    public List<int> NotifyUsers { get; set; }
}