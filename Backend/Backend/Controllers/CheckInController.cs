using AutoMapper;
using Backend.Authorization;
using Backend.Dtos.CheckInDtos;
using Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CheckInController: BaseController
{
    private readonly ICheckInService _checkInService;
    private readonly IMapper _mapper;

    public CheckInController(ICheckInService checkInService, IMapper mapper)
    {
        _checkInService = checkInService;
        _mapper = mapper;
    }
    
    [HttpGet("info/{slotId:int}")]
    public async Task<IActionResult> GetAllCheckIn(int slotId)
    {
        var result = await _checkInService.GetAllCheckIn(slotId);
        return Ok(result);
    }

    [HttpGet("{qrCodeString}")]
    public async Task<IActionResult> CheckIn(string qrCodeString)
    {
        return Ok(await _checkInService.CheckIn(qrCodeString));
    }
    
    [HttpPost]
    public async Task<IActionResult> CheckInConfirm(CheckInConfirmDtos input)
    {
        return Ok(await _checkInService.CheckInConfirm(input));
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _checkInService.Delete(id);
        return Ok(new { message = "Checkin deleted successfully" });
    }
}