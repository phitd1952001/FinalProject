using AutoMapper;
using Backend.Authorization;
using Backend.Dtos.CheckInDtos;
using Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScheduleController: BaseController
{

    public ScheduleController()
    {
       
    }
    
    [HttpGet]
    public async Task<IActionResult> Schedule()
    {
        return Ok();
    }
}