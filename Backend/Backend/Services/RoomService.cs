using AutoMapper;
using Backend.DbContext;
using Backend.Dtos.RoomDtos;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;

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
    }
}
