using Backend.Authorization;
using Backend.Dtos.RoomDtos;
using Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : BaseController
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomService.GetAllRooms();
            return Ok(rooms);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var findRoom = await _roomService.GetRoomById(id);
            return Ok(findRoom);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(CreateRoomRequest createRoomRequest)
        {
            var rooms = await _roomService.CreateRoom(createRoomRequest);
            return Ok(rooms);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateRoomRequest updatedRoomRequest)
        {
            var updateRoom = await _roomService.UpdateRoom(id ,updatedRoomRequest);
            return Ok(updateRoom);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _roomService.DeleteRoom(id);
            return Ok(new { message = "Account deleted successfully" });
        }
    }
}

