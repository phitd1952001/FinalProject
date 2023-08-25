using Backend.Dtos.Subject;
using Backend.Models;

namespace Backend.Services.IServices;

public interface ISubjectService
{
    Task<Subject> GetById(int id);
    Task<List<Subject>> GetAll();
    Task<Subject> Create(CreateSubjectRequest model);
    Task<Subject> Update(int id, UpdateSubjectRequest model);
    Task<Subject> Delete(int id);
}