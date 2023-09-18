using Backend.Authorization;
using Backend.Dtos.ExcelDtos;
using Backend.Dtos.SlotDtos;
using Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SlotController : BaseController
{
    private readonly ISlotService _slotService;

    public SlotController(ISlotService slotService)
    {
        _slotService = slotService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var slots = await _slotService.GetAllSlots();
        return Ok(slots);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var findSlot = await _slotService.GetSlotById(id);
        return Ok(findSlot);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSlotRequest createSlotRequest)
    {
        var slots = await _slotService.CreateSlot(createSlotRequest);
        return Ok(slots);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateSlotRequest updateSlotRequest)
    {
        var updateSlot = await _slotService.UpdateSlot(id, updateSlotRequest);
        return Ok(updateSlot);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _slotService.DeleteSlot(id);
        return Ok(new { message = "Slot deleted successfully" });
    }

    [HttpGet("available-fields")]
    public IActionResult GetAvailableFields()
    {
        List<string> availableFields = _slotService.GetFields();
        return Ok(availableFields);
    }

    [HttpPost("upload-excel")]
    public async Task<IActionResult> UploadExcelFile([FromForm] UploadExcelModel model)
    {
        try
        {
            if (model.file == null || model.file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var result = await _slotService.UploadExcel(model.file);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Handle exceptions and return an error response
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("final-upload-excel")]
    public async Task<IActionResult> ImportExcelFile([FromForm] ImportExcelModel model)
    {
        try
        {
            if (model.file == null || model.file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            await _slotService.ImportExcel(model.file, model.mapping);
            return Ok();
        }
        catch (Exception ex)
        {
            // Handle exceptions and return an error response
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}