using System.Data;
using AutoMapper;
using Backend.DbContext;
using Backend.Dtos.RoomDtos;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Backend.Services
{
    public class RoomService : IRoomService
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RoomService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;  
        }
        
        //List Room
        public async Task<IEnumerable<Room>> GetAllRooms()
        {
            var room = await _context.Rooms.ToListAsync();
            return room;
            
        }

        //Get room
        private async Task<Room> GetRoom(int id)
        {
            var rooms = await _context.Rooms.FindAsync(id);
            if (rooms == null) throw new KeyNotFoundException("Room not found!");
            return rooms;
        }

        //Find room by Id
        public async Task<Room> GetRoomById(int roomId)
        {
            var rooms = await GetRoom(roomId);
            return rooms;
        }

        //CreateRoom
        public async Task<Room> CreateRoom(CreateRoomRequest room)
        {
            if (_context.Rooms.Any(_ => _.Name == room.Name))
                throw new AppException("Room Name Already Existed");
            
            var createRoom = _mapper.Map<Room>(room);
            
            await _context.Rooms.AddAsync(createRoom);
            await _context.SaveChangesAsync();

            return createRoom;
        }

        //Update Room
        public async Task<Room> UpdateRoom(int roomId, UpdateRoomRequest updatedRoom)
        {
            if (_context.Rooms.Any(_ => _.Name == updatedRoom.Name && _.Id != roomId))
                throw new AppException("Room Name Already Existed");
            
            var existingRoom  = await GetRoom(roomId);
            existingRoom.Name = updatedRoom.Name;
            existingRoom.NumberOfSeat = updatedRoom.NumberOfSeat;
          
            await _context.SaveChangesAsync();

            return existingRoom;
        }

        //Delete Room
        public async Task DeleteRoom(int roomId)
        {
            var roomToDelete = await GetRoom(roomId);
            _context.Remove(roomToDelete);
            _context.SaveChanges();
        }
        
        public List<string> GetFields()
        {
            return new List<string>(){"Name", "NumberOfSeat"};
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

        public async Task<List<Room>> ImportExcel(IFormFile file, string mapping)
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
                var mappedData = new List<Room>();
                foreach (DataRow row in dt.Rows)
                {
                    var item = new Room();

                    foreach (var mappingEntry in userMapping)
                    {
                        var excelColumn = mappingEntry.Value;
                        var dataField = mappingEntry.Key;

                        // Map Excel data to the corresponding field
                        if (dt.Columns.Contains(excelColumn))
                        {
                            var value = row[excelColumn].ToString();

                            var propertyInfo = typeof(Room).GetProperty(dataField);
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

                var validData = new List<Room>();
                // Process and validate the data as needed and save
                foreach (var data in mappedData)
                {
                    if (CheckValidData(data))
                    {
                        validData.Add(data);
                    }
                }
                
                await _context.Rooms.AddRangeAsync(validData);
                await _context.SaveChangesAsync();

                return mappedData;
            }
        }

        private bool CheckValidData(Room room)
        {
            return !string.IsNullOrEmpty(room.Name) && room.NumberOfSeat > 0 && !_context.Rooms.Any(_ => _.Name == room.Name);
        }
    }
}
