﻿using System.Data;
using Backend.Dtos.RoomDtos;
using Backend.Models;

namespace Backend.Services.IServices
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllRooms();
        Task<Room> GetRoomById(int roomId);
        Task<Room> CreateRoom(CreateRoomRequest room);
        Task<Room> UpdateRoom(int roomId, UpdateRoomRequest updatedRoom);
        Task DeleteRoom(int roomId);
        List<string> GetFields();
        Task<List<Dictionary<string, string>>> UploadExcel(IFormFile file);
        Task<List<Room>> ImportExcel(IFormFile file, string mapping);
    }
}
