using System.Data;
using AutoMapper;
using Backend.DbContext;
using Backend.Dtos.Subject;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Backend.Services;

public class SubjectService : ISubjectService
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

        var subject = await GetById(id);
        _mapper.Map(model, subject);
        _context.Subjects.Update(subject);
        await _context.SaveChangesAsync();
        return subject;
    }

    public async Task<Subject> Delete(int id)
    {
        var deleteSubject = await GetById(id);
        _context.Subjects.Remove(deleteSubject);
        await _context.SaveChangesAsync();
        return deleteSubject;
    }

    public List<string> GetFields()
    {
        return new List<string>() { "SubjectCode", "Name", "Description" };
    }

    public async Task<List<Dictionary<string, string>>> UploadExcel(IFormFile file)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage(file.OpenReadStream()))
        {
            var worksheet = package.Workbook.Worksheets[0];

            // Extract data into a list of dictionaries
            var data = new List<Dictionary<string, string>>();
            for (var rowNumber = 1; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
            {
                var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                var rowData = new Dictionary<string, string>();
                foreach (var cell in row)
                {
                    rowData[cell.Start.Column.ToString()] = cell.Text;
                }

                data.Add(rowData);
            }

            return data;
        }
    }

    public async Task<List<Subject>> ImportExcel(IFormFile file, string mapping)
    {
        // Load the Excel file using EPPlus
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage(file.OpenReadStream()))
        {
            var worksheet = package.Workbook.Worksheets[0];

            // Extract data into a DataTable
            DataTable dt = new DataTable();
            foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
            {
                dt.Columns.Add(firstRowCell.Text);
            }

            for (var rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
            {
                var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                var newRow = dt.NewRow();
                var count = 0;
                foreach (var cell in row)
                {
                    newRow[count] = cell.Text;
                    count++;
                }

                dt.Rows.Add(newRow);
            }

            // Deserialize the mapping string into a C# object
            var userMapping =
                Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(mapping);

            // Map Excel columns to appropriate fields
            var mappedData = new List<Subject>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Subject();

                foreach (var mappingEntry in userMapping)
                {
                    var excelColumn = mappingEntry.Value;
                    var dataField = mappingEntry.Key;

                    // Map Excel data to the corresponding field
                    if (dt.Columns.Contains(excelColumn))
                    {
                        var value = row[excelColumn].ToString();

                        var propertyInfo = typeof(Subject).GetProperty(dataField);
                        propertyInfo?.SetValue(item, value);
                        
                    }
                }

                mappedData.Add(item);
            }

            var validData = new List<Subject>();
            // Process and validate the data as needed and save
            foreach (var data in mappedData)
            {
                if (CheckValidData(data))
                {
                    validData.Add(data);
                }
            }

            await _context.Subjects.AddRangeAsync(validData);
            await _context.SaveChangesAsync();

            return mappedData;
        }
    }

    private bool CheckValidData(Subject subject)
    {
        return !string.IsNullOrEmpty(subject.Name) && !string.IsNullOrEmpty(subject.SubjectCode) &&
               !_context.Subjects.Any(_ => _.SubjectCode == subject.SubjectCode);
    }
}