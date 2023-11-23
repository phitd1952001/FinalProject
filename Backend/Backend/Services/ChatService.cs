using Backend.DbContext;
using Backend.Dtos.ChatDtos;
using Backend.Services.IServices;

namespace Backend.Services;

public class ChatService : IChatService
{
    private readonly IImageServices _imageServices;
    private readonly ApplicationDbContext _dbContext;

    public ChatService(
        ApplicationDbContext dbContext,
        IImageServices imageServices
        )
    {
        _dbContext = dbContext;
        _imageServices = imageServices;
    }

    // public async Task<List<GetChatByUserIdResponse>> GetChatByUserId(string id)
    // {
    //     var chatUsers = await _chatUserRepository.FindListAsync(_ => _.UserId == id); // 
    //     var chatUsersContainChatWithUsersTakeChatId = chatUsers.Select(_ => _.ChatId).Distinct().ToList();
    //     var chatUsersContainChatWithUsers = await _chatUserRepository.FindListAsync(_ => chatUsersContainChatWithUsersTakeChatId.Contains(_.ChatId));
    //     var chatIds = chatUsers.Select(_ => _.ChatId).Distinct().ToList(); // 
    //     var chats = await _chatRepository.FindListAsync(_ => chatIds.Contains(_.Id)); // 
    //     var userIds = chatUsersContainChatWithUsers.Select(_ => _.UserId).Distinct().ToList(); // 
    //     var users = await _userRepository.FindListAsync(_ => userIds.Contains(_.Id) && !_.IsDeleted); // 
    //     var messages = await _messageRepository.FindListAsync(_ => chatIds.Contains(_.ChatId),20,0, -1); // 
    //
    //     var res = new List<GetChatByUserIdResponse>();
    //
    //     foreach (var chat in chats)
    //     {
    //         var chatUser = chatUsers.FirstOrDefault(_ => _.UserId == id && _.ChatId == chat.Id); // 
    //         var userIdsChatWith = chatUsersContainChatWithUsers.Where(_ => _.ChatId == chat.Id && _.UserId != id).Select(_ => _.UserId).ToList(); // 
    //         var usersChatWith = users.Where(_ => userIdsChatWith.Contains(_.Id)).ToList(); //
    //         var userResponseList = new List<UserResponse>();
    //         
    //         foreach (var user in usersChatWith)
    //         {
    //             var chatUserInUser = chatUsersContainChatWithUsers.FirstOrDefault(_ => _.ChatId == chat.Id && _.UserId == user.Id);
    //             if (chatUserInUser != null)
    //             {
    //                 var userResponse = new UserResponse()
    //                 {
    //                     Id = user.Id,
    //                     Avatar = string.Empty,
    //                     FirstName = user.Name.Split(" ")[1],
    //                     LastName = user.Name.Split(" ")[0],
    //                     Email = user.Email,
    //                     CreatedAt = user.CreateAt,
    //                     UpdatedAt = user.CreateAt,
    //                     ChatUser = new ChatUserResponse()
    //                     {
    //                         ChatId = chatUserInUser.ChatId,
    //                         UserId = chatUserInUser.UserId,
    //                         CreatedAt = chatUserInUser.CreateAt,
    //                         UpdatedAt = chatUserInUser.CreateAt
    //                     }
    //                 };
    //                 userResponseList.Add(userResponse);
    //             }
    //            
    //         }
    //
    //         var mess = messages.Where(_ => _.ChatId == chat.Id).ToList();
    //         var messageResponseList = new List<MessageResponse>();
    //         foreach (var m in mess)
    //         {
    //             var userWroteMessage = users.FirstOrDefault(_ => _.Id == m.FromUserId);
    //             if (userWroteMessage != null)
    //             {
    //                 var messageResponse = new MessageResponse()
    //                 {
    //                     Id = m.Id,
    //                     Type = m.Type,
    //                     Message = m.Message,
    //                     ChatId = m.ChatId,
    //                     FromUserId = m.FromUserId,
    //                     CreatedAt = m.CreateAt,
    //                     UpdatedAt = m.CreateAt,
    //                     User = new UserInMessage()
    //                     {
    //                         Id = userWroteMessage.Id,
    //                         Avatar = string.Empty,
    //                         FirstName = userWroteMessage.Name.Split(" ")[1],
    //                         LastName = userWroteMessage.Name.Split(" ")[0],
    //                         Email = userWroteMessage.Email,
    //                         CreatedAt = userWroteMessage.CreateAt,
    //                         UpdatedAt = userWroteMessage.CreateAt,
    //                     }
    //                 };
    //                 messageResponseList.Add(messageResponse);
    //             }
    //         }
    //         
    //         if (chatUser != null)
    //         {
    //             var result = new GetChatByUserIdResponse()
    //             {
    //                 Id = chat.Id,
    //                 Type = chat.Type,
    //                 CreatedAt = chat.CreateAt,
    //                 UpdatedAt = chat.CreateAt,
    //                 ChatUser = new ChatUserResponse()
    //                 {
    //                     UserId = chatUser.UserId,
    //                     ChatId = chatUser.ChatId,
    //                     CreatedAt = chatUser.CreateAt,
    //                     UpdatedAt = chatUser.CreateAt
    //                 },
    //                 Users = userResponseList,
    //                 Messages = messageResponseList,
    //             };
    //             res.Add(result);
    //         }
    //     }
    //     
    //     return res;
    // }
    //
    // public async Task<CreateChatResponse> Create(string partnerId, string userId)
    // {
    //     var chatUsers  = await _chatUserRepository.FindListAsync(_ => _.UserId == partnerId|| _.UserId == userId);
    //     var chatIdInChatUserOfPartner = chatUsers.Where(_ => _.UserId == partnerId).Select(_ => _.ChatId);
    //     var chatIdInChatUserOfUser = chatUsers.Where(_ => _.UserId == userId).Select(_ => _.ChatId);
    //     var isChatIdOfUserAndPartnerIntersect = chatIdInChatUserOfPartner.Intersect(chatIdInChatUserOfUser).Any();
    //     var isChatTypeDual = isChatIdOfUserAndPartnerIntersect ? 
    //         await _chatRepository.CountAsync(_ => chatIdInChatUserOfPartner.Intersect(chatIdInChatUserOfUser).Contains(_.Id) && _.Type == "dual") > 0 
    //         : false;
    //     var isUserAndPartnerAlreadyChat = isChatIdOfUserAndPartnerIntersect && isChatTypeDual;
    //
    //     if (isUserAndPartnerAlreadyChat)
    //         throw new AppException("Chat with this user already exists!");
    //     
    //     var chat = new Chat()
    //     {
    //         Type = "dual"
    //     };
    //
    //     await _chatRepository.InsertAsync(chat);
    //
    //     await _chatUserRepository.InsertRangeAsync(new List<ChatUser>()
    //     {
    //         new ChatUser()
    //         {
    //             UserId = userId,
    //             ChatId = chat.Id
    //         },
    //         new ChatUser()
    //         {
    //             UserId = partnerId,
    //             ChatId = chat.Id
    //         }
    //     });
    //
    //     var userInfos = await _userRepository.FindListAsync(_ => _.Id == userId || _.Id == partnerId);
    //     var userInfo = userInfos.FirstOrDefault(_ => _.Id == userId);
    //     var partnerInfo = userInfos.FirstOrDefault(_ => _.Id == partnerId);
    //     
    //     var forCreator = new CreateChatResponseModel()
    //     {
    //         Id = chat.Id,
    //         Type = "dual",
    //         Users = new List<UserInMessage>()
    //         {
    //             new UserInMessage()
    //             {
    //                 Id = partnerInfo.Id,
    //                 Avatar = string.Empty,
    //                 FirstName = partnerInfo.Name.Split(" ")[1],
    //                 LastName = partnerInfo.Name.Split(" ")[0],
    //                 Email = partnerInfo.Email,
    //                 CreatedAt = partnerInfo.CreateAt,
    //                 UpdatedAt = partnerInfo.CreateAt,
    //             }
    //         },
    //     };
    //     
    //     var forReceiver = new CreateChatResponseModel()
    //     {
    //         Id = chat.Id,
    //         Type = "dual",
    //         Users = new List<UserInMessage>()
    //         {
    //             new UserInMessage()
    //             {
    //                 Id = userInfo.Id,
    //                 Avatar = string.Empty,
    //                 FirstName = userInfo.Name.Split(" ")[1],
    //                 LastName = userInfo.Name.Split(" ")[0],
    //                 Email = userInfo.Email,
    //                 CreatedAt = userInfo.CreateAt,
    //                 UpdatedAt = userInfo.CreateAt,
    //             }
    //         },
    //     };
    //
    //     return new CreateChatResponse()
    //     {
    //         CreateChatResponseModels = new List<CreateChatResponseModel>() {forCreator, forReceiver}
    //     };
    // }
    //
    // public async Task<object> Messages(string id, int page = 1)
    // {
    //     var limit = 10;
    //     var offset = page > 1 ? page * limit : 0;
    //     var totalMessages = await _messageRepository.CountAsync(_ => _.ChatId == id);
    //     var totalPages = Math.Ceiling(Convert.ToDecimal(totalMessages / limit));
    //     var messages = await _messageRepository.FindListAsync(_ => _.ChatId == id, limit, offset,-1);
    //
    //     if (page > totalPages)
    //         return new { data = new {messages = new List<MessageResponse>() } };
    //
    //     var messageResponses = new List<MessageResponse>();
    //
    //     var fromUserIds = messages.Select(_ => _.FromUserId).ToList();
    //     var userInfos = await _userRepository.FindListAsync(_ =>fromUserIds.Contains(_.Id));
    //     foreach (var m in messages)
    //     {
    //         var userInfo = userInfos.FirstOrDefault(_ => _.Id == m.FromUserId);
    //         var messageResponse = new MessageResponse()
    //         {
    //             Id = m.Id,
    //             Type = m.Type,
    //             Message = m.Message,
    //             ChatId = m.ChatId,
    //             FromUserId = m.FromUserId,
    //             CreatedAt = m.CreateAt,
    //             UpdatedAt = m.CreateAt,
    //             User = new UserInMessage()
    //             {
    //                 Id = userInfo.Id,
    //                 Avatar = string.Empty,
    //                 FirstName = userInfo.Name.Split(" ")[1],
    //                 LastName = userInfo.Name.Split(" ")[0],
    //                 Email = userInfo.Email,
    //                 CreatedAt = userInfo.CreateAt,
    //                 UpdatedAt = userInfo.CreateAt,
    //             }
    //         };
    //         messageResponses.Add(messageResponse);
    //     }
    //
    //     return new {messages = messageResponses, pagination = new {page = page, totalPages = totalPages}};
    // }
    //
    // public async Task<object> AddUserToGroup(string chatId, string userId, string currentUserId)
    // {
    //     var chatUser = await _chatUserRepository.FindListAsync(_ => _.ChatId == chatId);
    //     if (chatUser.Any(_ => _.UserId == userId))
    //         throw new AppException("User already in the group!");
    //
    //     await _chatUserRepository.InsertAsync(new ChatUser()
    //     {
    //         ChatId = chatId,
    //         UserId = userId
    //     });
    //     
    //     var chat = await _chatRepository.FindAsync(chatId);
    //     if (chat.Type == "dual")
    //         await _chatRepository.UpdateOneAsync(_ => _.Id == chatId, _ => _.Type, "group");
    //     
    //     var messages = await _messageRepository.FindListAsync(_ => _.ChatId == chatId);
    //     
    //     var messageResponses = new List<MessageResponse>();
    //
    //     var chatUsers = await _chatUserRepository.FindListAsync(_ => _.ChatId == chatId);
    //     var fromUserIds = chatUsers.Select(_ => _.UserId).ToList();
    //     var userInfos = await _userRepository.FindListAsync(_ => fromUserIds.Contains(_.Id) || _.Id == userId);
    //     foreach (var m in messages)
    //     {
    //         var userInfo = userInfos.FirstOrDefault(_ => _.Id == m.FromUserId);
    //         var messageResponse = new MessageResponse()
    //         {
    //             Id = m.Id,
    //             Type = m.Type,
    //             Message = m.Message,
    //             ChatId = m.ChatId,
    //             FromUserId = m.FromUserId,
    //             CreatedAt = m.CreateAt,
    //             UpdatedAt = m.CreateAt,
    //             User = new UserInMessage()
    //             {
    //                 Id = userInfo.Id,
    //                 Avatar = string.Empty,
    //                 FirstName = userInfo.Name.Split(" ")[1],
    //                 LastName = userInfo.Name.Split(" ")[0],
    //                 Email = userInfo.Email,
    //                 CreatedAt = userInfo.CreateAt,
    //                 UpdatedAt = userInfo.CreateAt,
    //             }
    //         };
    //         messageResponses.Add(messageResponse);
    //     }
    //     
    //     var userResponseList = new List<UserResponse>();
    //     
    //     foreach (var user in userInfos)
    //     {
    //         var userResponse = new UserResponse()
    //         {
    //             Id = user.Id,
    //             Avatar = string.Empty,
    //             FirstName = user.Name.Split(" ")[1],
    //             LastName = user.Name.Split(" ")[0],
    //             Email = user.Email,
    //             CreatedAt = user.CreateAt,
    //             UpdatedAt = user.CreateAt,
    //             ChatUser = new ChatUserResponse()
    //             {
    //                 ChatId = chatId,
    //                 UserId = userId,
    //                 CreatedAt = chat.CreateAt,
    //                 UpdatedAt = chat.CreateAt
    //             }
    //         };
    //         userResponseList.Add(userResponse);
    //     }
    //
    //     var newChatter = userInfos.FirstOrDefault(_ => _.Id == userId);
    //
    //     return new
    //     {
    //         Chat = new
    //         {
    //             Id = chat.Id,
    //             Type = "group",
    //             CreatedAt = chat.CreateAt,
    //             UpdatedAt = chat.CreateAt,
    //             Users = userResponseList,
    //             Messages = messageResponses,   
    //         },
    //         NewChatter = new UserInMessage()
    //         {
    //             Id = newChatter.Id,
    //             Avatar = string.Empty,
    //             FirstName = newChatter.Name.Split(" ")[1],
    //             LastName = newChatter.Name.Split(" ")[0],
    //             Email = newChatter.Email,
    //             CreatedAt = newChatter.CreateAt,
    //             UpdatedAt = newChatter.CreateAt,
    //         }
    //         
    //     };
    // }
    //
    // public async Task<object> Delete(string chatId)
    // {
    //     var chatUser = await _chatUserRepository.FindListAsync(_ => _.ChatId == chatId);
    //     await _messageRepository.DeleteManyAsync(_ => _.ChatId == chatId);
    //     await _chatUserRepository.DeleteManyAsync(_ => _.ChatId == chatId);
    //     await _chatRepository.DeleteManyAsync(_ => _.Id == chatId);
    //     return new {ChatId = chatId, NotifyUsers = chatUser.Select(_ => _.UserId)};
    // }
    //
    // public async Task<object> LeaveCurrentChat(string chatId, string currentUserId)
    // {
    //     var chatUsers = await _chatUserRepository.FindListAsync(_ => _.ChatId == chatId);
    //
    //     if (chatUsers.Count == 2)
    //         throw new AppException("You cannot leave this chat");
    //
    //     if (chatUsers.Count == 3)
    //         await _chatRepository.UpdateOneAsync(_ => _.Id == chatId, _ => _.Type, "dual");
    //
    //     await _chatUserRepository.DeleteManyAsync(_ => _.ChatId == chatId && _.UserId == currentUserId);
    //     await _messageRepository.DeleteManyAsync(_ => _.ChatId == chatId && _.FromUserId == currentUserId);
    //
    //     return new
    //     {
    //         ChatId = chatId,
    //         UserId = currentUserId,
    //         CurrentUserId = currentUserId,
    //         NotifyUsers = chatUsers.Select(_=>_.UserId)
    //     };
    // }
    
    public object UpLoadImage(Stream image)
    {
        var uploadResult = _imageServices.UploadFile(image, Guid.NewGuid().ToString());
        return new {Url = uploadResult.Url.ToString()};
    }
}