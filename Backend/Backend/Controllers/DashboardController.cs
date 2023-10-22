using Backend.Authorization;
using Backend.Services;
using Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{ 
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class DashboardController: BaseController
    {
        private IDashBoardService _dashBoardService;

        public DashboardController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _dashBoardService.GetDashboard());
        }
    }
}
