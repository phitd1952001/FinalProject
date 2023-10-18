using Backend.Dtos.CheckInDtos;
using Backend.Models;

namespace Backend.Services.IServices;

public interface ICheckInService
{
    Task<IEnumerable<CheckInViewDetailDtos>> GetAllCheckIn(int slotId);
    Task<Object> CheckInConfirm(CheckInConfirmDtos input);
    Task<Object> CheckIn(string qrCodeString);
    Task Delete(int id);
}