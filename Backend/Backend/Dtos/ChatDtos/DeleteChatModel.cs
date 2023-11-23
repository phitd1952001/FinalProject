namespace Backend.Dtos.ChatDtos;

public class DeleteChatModel
{
    public int ChatId { get; set; }
    public List<int> NotifyUsers { get; set; }
}