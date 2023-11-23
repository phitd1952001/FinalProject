using Backend.Models;

namespace Backend.Dtos.ChatDtos;


public class CreateChatResponse
{
    public List<CreateChatResponseModel> CreateChatResponseModels { get; set; }
}

public class CreateChatResponseModel
{
    public string Id { get; set; }
    public string Type { get; set; }
    public List<UserInMessage> Users { get; set; }
    public List<ChatMessage> Messages = new List<ChatMessage>();
}
