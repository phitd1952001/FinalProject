using System.Data;
using AutoMapper;
using Backend.DbContext;
using Backend.Dtos.SlotDtos;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Backend.Services;

public class SlotService : ISlotService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SlotService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SlotResponse>> GetAllSlots()
    {
        var listSlots = await _context.Slots
            .Include(_ => _.Subject)
            .Include(_ => _.Room)
            .ToListAsync();
        var result = new List<SlotResponse>();
        foreach (var slots in listSlots)
        {
            var response = MappingSlotToSlotResponse(slots);
            result.Add(response);
        }

        return result;
    }

    private static SlotResponse MappingSlotToSlotResponse(Slot slots)
    {
        var response = new SlotResponse()
        {
            SlotId = slots.Id,
            Name = slots.Name,
            StartTime = slots.StartTime,
            Duration = slots.Duration,
            SubjectId = slots.SubjectId,
            SubjectName = slots.Subject.Name,
            RoomId = slots.RoomId,
            RoomName = slots.Room.Name
        };

        return response;
    }

    private async Task<SlotResponse> MappingSlotToSlotResponseNotIncluded(Slot slots, int subjectId, int roomId)
    {
        var subject = await _context.Subjects.FirstOrDefaultAsync(_ => _.Id == subjectId);
        var room = await _context.Rooms.FirstOrDefaultAsync(_ => _.Id == roomId);

        var response = new SlotResponse()
        {
            SlotId = slots.Id,
            Name = slots.Name,
            StartTime = slots.StartTime,
            Duration = slots.Duration,
            SubjectId = subject.Id,
            SubjectName = subject.Name,
            RoomId = room.Id,
            RoomName = room.Name
        };

        return response;
    }

    public async Task<SlotResponse> GetSlotById(int slotId)
    {
        var slot = await _context.Slots
            .Include(_ => _.Subject)
            .Include(_ => _.Room)
            .FirstOrDefaultAsync(_ => _.Id == slotId);
        var response = MappingSlotToSlotResponse(slot);
        return response;
    }

    public async Task<SlotResponse> CreateSlot(CreateSlotRequest model)
    {
        if (await _context.Slots.AnyAsync(_ => _.Name.Trim().ToLower() == model.Name.Trim().ToLower()))
            throw new AppException("This Name is in used");
        if (await _context.Slots.AnyAsync(_ =>
                _.StartTime <= model.StartTime && model.StartTime <= _.StartTime.AddMinutes(_.Duration) &&
                _.RoomId == model.RoomId && _.SubjectId == model.SubjectId))
            throw new AppException("This Room is in used");

        var slot = _mapper.Map<Slot>(model);
        await _context.Slots.AddAsync(slot);
        await _context.SaveChangesAsync();

        return await MappingSlotToSlotResponseNotIncluded(slot, model.SubjectId, model.RoomId);
    }

    public async Task<SlotResponse> UpdateSlot(int slotId, UpdateSlotRequest model)
    {
        if (await _context.Slots.AnyAsync(_ =>
                _.Name.Trim().ToLower() == model.Name.Trim().ToLower() && _.Id != slotId))
            throw new AppException("This Name is in used");
        if (await _context.Slots.AnyAsync(_ => _.StartTime <= model.StartTime
                                               && model.StartTime <= _.StartTime.AddMinutes(_.Duration)
                                               && _.RoomId == model.RoomId
                                               && _.SubjectId == model.SubjectId
                                               && _.Id != slotId))
            throw new AppException("This Room is in used");

        var slot = await _context.Slots.FirstOrDefaultAsync(_ => _.Id == slotId);
        _mapper.Map(model, slot);

        await _context.SaveChangesAsync();

        return await MappingSlotToSlotResponseNotIncluded(slot, model.SubjectId, model.RoomId);
    }

    public async Task DeleteSlot(int slotId)
    {
        var slot = await _context.Slots
            .FirstOrDefaultAsync(_ => _.Id == slotId);
        _context.Slots.Remove(slot);
        _context.SaveChanges();
    }

    public List<string> GetFields()
    {
        return new List<string>() { "Name", "StartTime", "Duration", "SubjectCode", "RoomName" };
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

    public async Task ImportExcel(IFormFile file, string mapping)
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
            var mappedData = new List<ExcelMappingModel>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new ExcelMappingModel();

                foreach (var mappingEntry in userMapping)
                {
                    var excelColumn = mappingEntry.Value;
                    var dataField = mappingEntry.Key;

                    // Map Excel data to the corresponding field
                    if (dt.Columns.Contains(excelColumn))
                    {
                        var value = row[excelColumn].ToString();

                        var propertyInfo = typeof(ExcelMappingModel).GetProperty(dataField);
                        if (propertyInfo?.PropertyType == typeof(int))
                        {
                            int intValue;
                            if (int.TryParse(value, out intValue))
                            {
                                // Successfully converted to int
                                propertyInfo.SetValue(item, intValue);
                            }
                            else
                            {
                                propertyInfo.SetValue(item, -1);
                            }
                        }
                        else
                        {
                            propertyInfo?.SetValue(item, value);
                        }
                    }
                }

                mappedData.Add(item);
            }

            var subjectCodes = mappedData.Select(_ => _.SubjectCode).Distinct().ToList();
            var subjects = _context.Subjects.Where(_ => subjectCodes.Contains(_.SubjectCode)).ToList();
            var roomNames = mappedData.Select(_ => _.RoomName).Distinct().ToList();
            var rooms = _context.Rooms.Where(_ => roomNames.Contains(_.Name)).ToList();

            var slots = await _context.Slots.ToListAsync();

            var validData = new List<Slot>();
            // Process and validate the data as needed and save
            foreach (var data in mappedData)
            {
                var roomId = rooms.FirstOrDefault(_ => _.Name == data.RoomName)?.Id;
                var subjectId = subjects.FirstOrDefault(_ => _.SubjectCode == data.SubjectCode)?.Id;

                if (roomId != null && subjectId != null)
                {
                    string format = "MM/dd/yyyy-HH:mm";
                    var startTime = DateTime.ParseExact(data.StartTime, format, null);
                    var slot = new Slot()
                    {
                        Name = data.Name,
                        Duration = data.Duration,
                        RoomId = roomId.Value,
                        StartTime = startTime,
                        SubjectId = subjectId.Value,
                    };

                    if (CheckValidData(slot, slots, validData))
                    {
                        validData.Add(slot);
                    }
                }
            }

            await _context.Slots.AddRangeAsync(validData);
            await _context.SaveChangesAsync();
        }
    }
    

    private bool CheckValidData(Slot slot, List<Slot> slots, List<Slot> validData)
    {
        var isNameInUsed = slots.Any(_ => _.Name.Trim().ToLower() == slot.Name.Trim().ToLower()) && validData.Any(_ => _.Name.Trim().ToLower() == slot.Name.Trim().ToLower());
        
        var isSlotInUsed = validData.Any(_ => _.StartTime <= slot.StartTime
                                         && slot.StartTime <= _.StartTime.AddMinutes(_.Duration)
                                         && _.RoomId == slot.RoomId
                                         && _.SubjectId == slot.SubjectId)
                            && slots.Any(_ => _.StartTime <= slot.StartTime
                                              && slot.StartTime <= _.StartTime.AddMinutes(_.Duration)
                                              && _.RoomId == slot.RoomId
                                              && _.SubjectId == slot.SubjectId);

        return !isNameInUsed && !isSlotInUsed;
    }
}