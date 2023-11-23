using System.Data;
using AutoMapper;
using Backend.DbContext;
using Backend.Dtos.ClassDtos;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Backend.Services;

public class ClassService : IClassService
{
    private readonly ApplicationDbContext _context;

    public ClassService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClassResponse>> GetAllClasses()
    {
        var classes = await _context.Classes
            .Include(_ => _.Subject)
            .Include(_ => _.Account)
            .ToListAsync();

        var result = MappingClassResponses(classes);
        return result;
    }

    private static List<ClassResponse> MappingClassResponses(List<Class> classes)
    {
        var result = new List<ClassResponse>();
        foreach (var @class in classes)
        {
            var response = MappingClassToClassResponse(@class);
            result.Add(response);
        }

        return result;
    }

    private static ClassResponse MappingClassToClassResponse(Class @class)
    {
        var response = new ClassResponse()
        {
            ClassId = @class.Id,
            Email = @class.Account.Email,
            SubjectCode = @class.Subject.SubjectCode,
            SubjectId = @class.SubjectId,
            UserId = @class.UserId
        };

        return response;
    }

    public async Task<ClassResponse> GetClassById(int classId)
    {
        var @class = await _context.Classes
            .Include(_ => _.Subject)
            .Include(_ => _.Account)
            .FirstOrDefaultAsync(_ => _.Id == classId);

        var response = MappingClassToClassResponse(@class);
        return response;
    }

    public async Task<ClassResponse> CreateClass(CreateClassRequest model)
    {
        var subject = await _context.Subjects.FirstOrDefaultAsync(_ =>
            _.SubjectCode.Trim().ToLower() == model.SubjectCode.Trim().ToLower());
        var user = await _context.Accounts.FirstOrDefaultAsync(_ =>
            _.Email.Trim().ToLower() == model.Email.Trim().ToLower());
        if (subject == null || user == null)
            throw new AppException("User or subject not found");

        var isUserJoinSubject = await _context.Classes.AnyAsync(_ => _.SubjectId == subject.Id && _.UserId == user.Id);
        if (isUserJoinSubject)
            throw new AppException("User " + model.Email + " is already join " + model.SubjectCode);

        var @class = new Class()
        {
            UserId = user.Id,
            SubjectId = subject.Id
        };

        await _context.Classes.AddAsync(@class);
        await _context.SaveChangesAsync();

        return MappingClassToClassResponse(@class);
    }

    public async Task<ClassResponse> UpdateClass(int classId, UpdateClassRequest model)
    {
        var subject = await _context.Subjects.FirstOrDefaultAsync(_ =>
            _.SubjectCode.Trim().ToLower() == model.SubjectCode.Trim().ToLower());
        var user = await _context.Accounts.FirstOrDefaultAsync(_ =>
            _.Email.Trim().ToLower() == model.Email.Trim().ToLower());
        if (subject == null || user == null)
            throw new AppException("User or subject not found");

        var isUserJoinSubject =
            await _context.Classes.AnyAsync(_ => _.SubjectId == subject.Id && _.UserId == user.Id && _.Id == classId);
        if (isUserJoinSubject)
            throw new AppException("User " + model.Email + " is already join " + model.SubjectCode);

        var @class = await _context.Classes.FirstOrDefaultAsync(_ => _.Id == classId);

        @class.UserId = user.Id;
        @class.SubjectId = subject.Id;

        await _context.SaveChangesAsync();

        return MappingClassToClassResponse(@class);
    }

    public async Task DeleteClass(int classId)
    {
        var @class = await _context.Classes
            .FirstOrDefaultAsync(_ => _.Id == classId);

        _context.Remove(@class);
        _context.SaveChanges();
    }

    public List<string> GetFields()
    {
        return new List<string>() { "Email", "SubjectCode" };
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

    public async Task<List<ClassResponse>> ImportExcel(IFormFile file, string mapping)
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
            var mappedData = new List<CreateClassRequest>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new CreateClassRequest();

                foreach (var mappingEntry in userMapping)
                {
                    var excelColumn = mappingEntry.Value;
                    var dataField = mappingEntry.Key;

                    // Map Excel data to the corresponding field
                    if (dt.Columns.Contains(excelColumn))
                    {
                        var value = row[excelColumn].ToString();

                        var propertyInfo = typeof(CreateClassRequest).GetProperty(dataField);
                        propertyInfo?.SetValue(item, value);
                        
                    }
                }

                mappedData.Add(item);
            }

            var subjectCodes = mappedData.Select(_ => _.SubjectCode).Distinct().ToList();
            var subjects = _context.Subjects.Where(_ => subjectCodes.Contains(_.SubjectCode)).ToList();
            var userEmails = mappedData.Select(_ => _.Email).Distinct().ToList();
            var users = _context.Accounts.Where(_ => userEmails.Contains(_.Email)).ToList();

            var classes = await _context.Classes
                .Include(_ => _.Subject)
                .Include(_ => _.Account)
                .ToListAsync();
            
            var validData = new List<Class>();
            // Process and validate the data as needed and save
            foreach (var data in mappedData)
            {
                var userId = users.FirstOrDefault(_ => _.Email == data.Email)?.Id;
                var subjectId = subjects.FirstOrDefault(_ => _.SubjectCode == data.SubjectCode)?.Id;

                if (userId != null && subjectId != null)
                {
                    var @class = new Class()
                    {
                        UserId = userId.Value,
                        SubjectId =subjectId.Value, 
                    };
                
                    if (CheckValidData(@class, classes, validData))
                    {
                        validData.Add(@class);
                    }
                }
               
            }

            await _context.Classes.AddRangeAsync(validData);
            await _context.SaveChangesAsync();

            var result = MappingClassResponses(validData);
            return result;
        }
    }

    private bool CheckValidData(Class @class, List<Class> classes, List<Class> validData)
    {

        var isUserJoinSubjectInDbList = classes.Any(_=>_.SubjectId == @class.SubjectId && _.UserId == @class.UserId);
        var isUserJoinSubjectInValidDataList = validData.Any(_=>_.SubjectId == @class.SubjectId && _.UserId == @class.UserId);

        return !isUserJoinSubjectInDbList && !isUserJoinSubjectInValidDataList;
    }
}