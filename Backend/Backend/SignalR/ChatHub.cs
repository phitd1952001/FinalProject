using Backend.DbContext;
using Backend.Dtos.ChatDtos;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Backend.SignalR;

public class ChatHub : Hub
{
    private readonly IDictionary<int, UserConnection> users;
    private readonly IDictionary<string, int> userSockets;
    private readonly ApplicationDbContext _context;

    public ChatHub(ApplicationDbContext context,
        IDictionary<int, UserConnection> _users, 
        IDictionary<string, int> _userSockets)
    {
        _context = context;
        users = _users;
        userSockets = _userSockets;
    }

    public async Task Join(UserResponse user)
    {
        var sockets = new HashSet<string>();

        if (users.TryGetValue(user.Id, out var existingUser))
        {
            if (existingUser.Sockets?.Add(Context.ConnectionId) == true)
            {
                userSockets[Context.ConnectionId] = user.Id;
                sockets = existingUser.Sockets;
            }
        }
        else
        {
            var newUser = new UserConnection { Id = user.Id, Sockets = new HashSet<string> { Context.ConnectionId } };
            users[user.Id] = newUser;
            sockets.Add(Context.ConnectionId);
            userSockets[Context.ConnectionId] = user.Id;
        }

        var onlineFriends = new HashSet<int>();
    
        var chatters = await GetChattersAsync(user.Id);

        foreach (var chatterId in chatters)
        {
            if (users.TryGetValue(chatterId, out var chatter))
            {
                foreach (var socket in chatter.Sockets)
                {
                    try
                    {
                        await Clients.Client(socket).SendAsync("online", user);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception appropriately
                    }
                }

                onlineFriends.Add(chatter.Id);
            }
        }

        try
        {
            await Task.WhenAll(sockets.Select(socket => Clients.Client(socket).SendAsync("friends", onlineFriends)));
        }
        catch (Exception ex)
        {
            // Log or handle the exception appropriately
        }
    }


    public async Task Message(MessageSocket message)
    {
        var sockets = new HashSet<string>();

        if (users.TryGetValue(message.FromUser.Id, out var fromUser))
        {
            sockets.UnionWith(fromUser.Sockets);
        }

        foreach (var id in message.ToUserId)
        {
            if (users.TryGetValue(id, out var toUser))
            {
                sockets.UnionWith(toUser.Sockets);
            }
        }

        try
        {
            var msg = new ChatMessage()
            {
                Type = message.Type,
                FromUserId = message.FromUser.Id,
                ChatId = message.ChatId,
                Message = message.Message,
            };

            _context.ChatMessages.Add(msg);
            await _context.SaveChangesAsync();

            var messageResponse = new MessageResponse()
            {
                Id = msg.Id,
                ChatId = msg.ChatId,
                CreatedAt = msg.CreateAt,
                UpdatedAt = msg.CreateAt,
                Type = msg.Type,
                FromUserId = message.FromUser.Id,
                User = message.FromUser,
                Message = msg.Message
            };

            var sendTasks = sockets.Select(async socket =>
            {
                try
                {
                    await Clients.Client(socket).SendAsync("received", messageResponse);
                }
                catch (Exception ex)
                {
                    // Log or handle the exception appropriately
                }
            });

            await Task.WhenAll(sendTasks);
        }
        catch (Exception ex)
        {
            // Log or handle the exception appropriately
        }
    }


    public async Task Typing(TypingModel model)
    {
        foreach (var id in model.ToUserId)
        {
            if (users.TryGetValue(id, out var user))
            {
                var userSockets = user.Sockets;

                var sendTasks = userSockets.Select(async socket =>
                {
                    try
                    {
                        await Clients.Client(socket).SendAsync("typing", model);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception appropriately
                    }
                });

                await Task.WhenAll(sendTasks);
            }
        }
    }


    public async Task AddFriend(AddFriendModel model)
    {
        try
        {
            const string onlineStatus = "online";

            async Task NotifyUserAboutNewChat(ChatModel chat, int userId)
            {
                if (users.TryGetValue(userId, out var user))
                {
                    chat.Users[0].Status = onlineStatus;
                    var userSockets = user.Sockets;

                    var sendTasks = userSockets.Select(async socket =>
                    {
                        try
                        {
                            await Clients.Client(socket).SendAsync("new-chat", chat);
                        }
                        catch (Exception ex)
                        {
                            // Log or handle the exception appropriately
                        }
                    });

                    await Task.WhenAll(sendTasks);
                }
            }

            foreach (var chat in model.Chats)
            {
                chat.Messages = new List<object>();
            }

            if (users.ContainsKey(model.Chats[1].Users[0].Id))
            {
                await NotifyUserAboutNewChat(model.Chats[0], model.Chats[1].Users[0].Id);
            }

            if (users.ContainsKey(model.Chats[0].Users[0].Id))
            {
                await NotifyUserAboutNewChat(model.Chats[1], model.Chats[0].Users[0].Id);
            }
        }
        catch (Exception e)
        {
            // Log or handle the exception appropriately
        }
    }


    public async Task AddUserToGroup(AddUserToGroupModel model)
    {
        if (users.ContainsKey(model.NewChatter.Id))
        {
            model.NewChatter.Status = "online";
        }

        // Update the status of old users and send messages to them
        foreach (var user in model.Chat.Users)
        {
            if (users.ContainsKey(user.Id))
            {
                user.Status = "online";
                var userSockets = users[user.Id].Sockets;

                foreach (var socket in userSockets)
                {
                    await Clients.Client(socket).SendAsync("added-user-to-group", new
                    {
                        chat = model.Chat,
                        chatters = new List<UserInChatSignalR> { model.NewChatter }
                    });
                }
            }
        }

        // Send the updated chat to the new chatter
        if (users.ContainsKey(model.NewChatter.Id))
        {
            var newChatterSockets = users[model.NewChatter.Id].Sockets;

            foreach (var socket in newChatterSockets)
            {
                await Clients.Client(socket).SendAsync("added-user-to-group", new
                {
                    chat = model.Chat,
                    chatters = model.Chat.Users
                });
            }
        }
    }

    public async Task LeaveCurrentChat(LeaveCurrentChatModel model)
    {
        foreach (var id in model.NotifyUsers)
        {
            if (users.ContainsKey(id))
            {
                var userSockets = users[id].Sockets;

                foreach (var socket in userSockets)
                {
                    await Clients.Client(socket).SendAsync("remove-user-from-chat", new
                    {
                        ChatId = model.ChatId,
                        UserId = model.UserId,
                        CurrentUserId = model.CurrentUserId
                    });
                }
            }
        }
    }

    public async Task DeleteChat(DeleteChatModel model)
    {
        try
        {
            async Task NotifyUserAboutDeletedChat(int userId)
            {
                if (users.TryGetValue(userId, out var user))
                {
                    var userSockets = user.Sockets;

                    var sendTasks = userSockets.Select(async socket =>
                    {
                        try
                        {
                            await Clients.Client(socket).SendAsync("delete-chat", model.ChatId);
                        }
                        catch (Exception ex)
                        {
                            // Log or handle the exception appropriately
                        }
                    });

                    await Task.WhenAll(sendTasks);
                }
            }

            foreach (var userId in model.NotifyUsers)
            {
                if (users.ContainsKey(userId))
                {
                    await NotifyUserAboutDeletedChat(userId);
                }
            }
        }
        catch (Exception e)
        {
            // Log or handle the exception appropriately
        }
    }
    
    public async override Task OnDisconnectedAsync(Exception exception)
    {
        if (userSockets.TryGetValue(Context.ConnectionId, out var userId))
        {
            if (users.TryGetValue(userId, out var user))
            {
                if (user.Sockets.Count > 1)
                {
                    // Remove the disconnected socket from the user's list of sockets
                    user.Sockets.Remove(Context.ConnectionId);
                }
                else
                {
                    var chatters = await GetChattersAsync(userId);

                    foreach (var chatterId in chatters)
                    {
                        if (users.TryGetValue(chatterId, out var chatter))
                        {
                            foreach (var socket in chatter.Sockets)
                            {
                                try
                                {
                                    await Clients.Client(socket).SendAsync("offline", user);
                                }
                                catch (Exception)
                                {
                                    // Log or handle the exception appropriately
                                }
                            }
                        }
                    }

                    // Remove the disconnected user from the dictionaries
                    userSockets.Remove(Context.ConnectionId);
                    users.Remove(userId);
                }
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task<IEnumerable<int>> GetChattersAsync(int userId)
    {
        // Use asynchronous operations if supported by _context
        var chatUsers = await _context.ChatUsers
            .Where(cu => cu.UserId == userId)
            .ToListAsync();

        var chatIds = chatUsers.Select(cu => cu.ChatId);

        var userIdsChatWith = await _context.ChatUsers
            .Where(cu => chatIds.Contains(cu.ChatId) && cu.UserId != userId)
            .Select(cu => cu.UserId)
            .ToListAsync();

        return userIdsChatWith;
    }
}
