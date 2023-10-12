using Backend.Dtos.CheckInDtos;

namespace Backend.Services.IServices;

public interface ICheckInService
{
    Task<Object> CheckInConfirm(CheckInConfirmDtos input);
    Task<Object> CheckIn(string qrCodeString);
}