using Backend.Dtos.SettingDtos;
using Backend.Models;

namespace Backend.Services.IServices
{
    public interface ISettingService
    {
        Task<IEnumerable<Setting>> GetAllSettings();
        Task<Setting> GetSettingById(int settingId);
        Task<Setting> CreateSetting(SettingUpsertDtos settingUpsert);
        Task<Setting> UpdateSetting(int settingId, SettingUpsertDtos settingUpsert);
        Task DeleteSetting(int settingId);
    }
}
