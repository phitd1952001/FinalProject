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

    public CheckInController(ICheckInService checkInService)
    {
        _checkInService = checkInService;
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
}