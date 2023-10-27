using AutoMapper;
using Backend.Authorization;
using Backend.BackgroundServices;
using Backend.Dtos.CheckInDtos;
using Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScheduleController: BaseController
{
    private IAutomationGenerateSchedule _automation;
    private BackgroundWorkerQueue _backgroundWorkerQueue;

    public ScheduleController(IAutomationGenerateSchedule automation, BackgroundWorkerQueue backgroundWorkerQueue)
    {
        _automation = automation;
        _backgroundWorkerQueue = backgroundWorkerQueue;
    }
    
    [HttpGet]
    public async Task<IActionResult> Schedule()
    {
        _backgroundWorkerQueue.QueueBackgroundWorkItem(async token =>
        {
            await _automation.Schedule();
        });

        return Ok();
    }
}