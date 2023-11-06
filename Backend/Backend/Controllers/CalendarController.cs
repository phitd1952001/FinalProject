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
public class CalendarController: BaseController
{
    public CalendarController()
    {
     
    }
}