using Backend.Authorization;
using Backend.Dtos.ChatDtos;
using Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatsController : BaseController
{
    private readonly IChatService _chatService;

    public ChatsController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var res = await _chatService.GetChatByUserId(Account.Id);
        return Ok(res);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateChatRequest model)
    {
        var res = await _chatService.Create(model.PartnerId, Account.Id);
        return Ok(res.CreateChatResponseModels);
    }
        
    [HttpGet("messages/{id:int}/{page:int}")]
    public async Task<IActionResult> GetAll(int id, int page)
    {
        var res = await _chatService.Messages(id, page);
        return Ok(res);
    }
        
    [HttpPost("upload-image")]
    public IActionResult UpLoadImage([FromForm] UploadImageRequest model)
    {
        using var stream = model.image.OpenReadStream();
            
        var res = _chatService.UpLoadImage(stream);
        return Ok(res);
    }
        
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await _chatService.Delete(id);
        return Ok(res);
    }
        
    [HttpPost("add-user-to-group")]
    public async Task<IActionResult> AddUserToGroup(AddUserToGroupRequest model)
    {
        var res = await _chatService.AddUserToGroup(model.ChatId, model.UserId, Account.Id);
        return Ok(res);
    }
        
    [HttpPost("leave-current-chat")]
    public async Task<IActionResult> LeaveGroup(LeaveGroupRequest model)
    {
        var res = await _chatService.LeaveCurrentChat(model.ChatId, Account.Id);
        return Ok(res);
    }
}