using Backend.Authorization;
using Backend.Dtos.Subject;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SubjectsController : BaseController
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }
    // GET
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _subjectService.GetAll();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var subject = await _subjectService.GetById(id);
        return Ok(subject);
    }

    [Authorize(Role.Admin, Role.Staff)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateSubjectRequest model)
    {
        var crateSubject = await _subjectService.Create(model);
        return Ok(crateSubject);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateSubjectRequest model)
    {
        var updateSubject = await _subjectService.Update(id, model);
        return Ok(updateSubject);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleteSubject = await _subjectService.Delete(id);
        return Ok(new { message = "Subject deleted successfully" });
    }
}