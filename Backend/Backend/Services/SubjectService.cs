using AutoMapper;
using Backend.DbContext;
using Backend.Dtos.Subject;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class SubjectService: ISubjectService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SubjectService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<Subject> GetById(int id)
    {
        var result = await _context.Subjects.FindAsync(id);
        return result;
    }

    public async Task<List<Subject>> GetAll()
    {
        var listSubject = await _context.Subjects.ToListAsync();
        return listSubject;
    }

    public async Task<Subject> Create(CreateSubjectRequest model)
    {
        if (await _context.Subjects.AnyAsync(_ => _.SubjectCode == model.SubjectCode))
        {
            throw new AppException("Subject Code Is Already Existed");
        }
        var subject = _mapper.Map<Subject>(model);
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();
        return subject;
    }

    public async Task<Subject> Update(int id, UpdateSubjectRequest model)
    {
        if (await _context.Subjects.AnyAsync(_ => _.SubjectCode == model.SubjectCode && _.Id != id))
        {
            throw new AppException("Subject Code Is Already Existed");
        }
        var subject = await _context.Subjects.FindAsync(id);
        _mapper.Map(model, subject);
        _context.Subjects.Update(subject);
        await _context.SaveChangesAsync();
        return subject;
    }

    public async Task<Subject> Delete(int id)
    {
        var deleteSubject = await _context.Subjects.FindAsync(id);
        _context.Subjects.Remove(deleteSubject);
        await _context.SaveChangesAsync();
        return deleteSubject;
    }
}