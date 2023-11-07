using Backend.Authorization;
using Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CalendarController: BaseController
{
    private ICalendarService _calendarService;

    public CalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllScheduler()
    {
        return Ok(await _calendarService.GetAllScheduler());
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetSchedulerByUserId()
    {
        return Ok(await _calendarService.GetSchedulerByUserId(Account.Id));
    }
}