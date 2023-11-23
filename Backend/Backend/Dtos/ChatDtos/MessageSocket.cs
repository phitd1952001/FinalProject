namespace Backend.Dtos.ChatDtos;

public class MessageSocket
{
    public string Type { get; set; }
    public UserInMessage FromUser  { get; set; }
    public List<int> ToUserId  { get; set; }
    public int ChatId  { get; set; }
    public string Message  { get; set; }
}