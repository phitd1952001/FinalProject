using Backend.Authorization;
using Backend.Dtos.ClassDtos;
using Backend.Dtos.ExcelDtos;
using Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClassController : BaseController
{
    private readonly IClassService _classService;

    public ClassController(IClassService classService)
    {
        _classService = classService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var classes = await _classService.GetAllClasses();
        return Ok(classes);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var findClass = await _classService.GetClassById(id);
        return Ok(findClass);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClassRequest createClassRequest)
    {
        var classes = await _classService.CreateClass(createClassRequest);
        return Ok(classes);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateClassRequest updatedClassRequest)
    {
        var updateClass = await _classService.UpdateClass(id, updatedClassRequest);
        return Ok(updateClass);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _classService.DeleteClass(id);
        return Ok(new { message = "Class deleted successfully" });
    }

    [HttpGet("available-fields")]
    public IActionResult GetAvailableFields()
    {
        List<string> availableFields = _classService.GetFields();
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

            var result = await _classService.UploadExcel(model.file);
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

            var result = await _classService.ImportExcel(model.file, model.mapping);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Handle exceptions and return an error response
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}