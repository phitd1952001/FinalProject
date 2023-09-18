using Backend.Dtos.SlotDtos;
using Backend.Models;

namespace Backend.Services.IServices;

public interface ISlotService
{
    Task<IEnumerable<SlotResponse>> GetAllSlots();
    Task<SlotResponse> GetSlotById(int slotId);
    Task<SlotResponse> CreateSlot(CreateSlotRequest model);
    Task<SlotResponse> UpdateSlot(int slotId, UpdateSlotRequest model);
    Task DeleteSlot(int slotId);
    List<string> GetFields();
    Task<List<Dictionary<string, string>>> UploadExcel(IFormFile file);
    Task ImportExcel(IFormFile file, string mapping);
}