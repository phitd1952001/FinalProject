using System.Collections;
using Backend.Dtos.Subject;
using Backend.Models;

namespace Backend.Services.IServices;

public interface ISubjectService
{
    Task<IEnumerable<Subject>> GetAll();
    Task<Subject> GetById(int id);
    Task<Subject> Create(CreateSubjectRequest model);
    Task<Subject> Update(int id, UpdateSubjectRequest model);
    Task<Subject> Delete(int id);
    //Excel
    List<string> GetFields();
    Task<List<Dictionary<string, string>>> UploadExcel(IFormFile file);
    Task<List<Subject>> ImportExcel(IFormFile file, string mapping);
}