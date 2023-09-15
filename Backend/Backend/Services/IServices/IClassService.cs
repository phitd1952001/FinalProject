using Backend.Dtos.ClassDtos;

namespace Backend.Services.IServices;

public interface IClassService
{
    Task<IEnumerable<ClassResponse>> GetAllClasses();
    Task<ClassResponse> GetClassById(int classId);
    Task<ClassResponse> CreateClass(CreateClassRequest model);
    Task<ClassResponse> UpdateClass(int classId, UpdateClassRequest model);
    Task DeleteClass(int classId);
    List<string> GetFields();
    Task<List<Dictionary<string, string>>> UploadExcel(IFormFile file);
    Task<List<ClassResponse>> ImportExcel(IFormFile file, string mapping);
}