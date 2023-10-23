using Backend.Authorization;
using Backend.Dtos.RoomDtos;
using Backend.Dtos.SettingDtos;
using Backend.Services;
using Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SettingController : BaseController
    {
        private ISettingService _settingService;

        public SettingController(ISettingService settingService) {
            _settingService = settingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var settings = await _settingService.GetAllSettings();
            return Ok(settings);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var findSetting = await _settingService.GetSettingById(id);
            return Ok(findSetting);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SettingUpsertDtos createSettingRequest)
        {
            var setting = await _settingService.CreateSetting(createSettingRequest);
            return Ok(setting);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, SettingUpsertDtos updatedSettingRequest)
        {
            var updateSetting = await _settingService.UpdateSetting(id, updatedSettingRequest);
            return Ok(updateSetting);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _settingService.DeleteSetting(id);
            return Ok(new { message = "Setting deleted successfully" });
        }
    }
}
