using Backend.Dtos.ChatDtos;

namespace Backend.Services.IServices;

public interface IChatService
{
    Task<List<GetChatByUserIdResponse>> GetChatByUserId(int id);
    Task<CreateChatResponse> Create(int partnerId, int userId);
    Task<object> Messages(int id, int page = 1);
    Task<object> AddUserToGroup(int chatId, int userId, int currentUserId);
    Task<object> Delete(int chatId);
    Task<object> LeaveCurrentChat(int chatId, int currentUserId);
    object UpLoadImage(Stream image);
}