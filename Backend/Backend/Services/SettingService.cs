using AutoMapper;
using Backend.DbContext;
using Backend.Dtos.SettingDtos;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class SettingService : ISettingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SettingService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Setting> CreateSetting(SettingUpsertDtos settingUpsert)
        {
            if (_context.Settings.Any())
                throw new AppException("Settings Already Existed");

            var createSetting = _mapper.Map<Setting>(settingUpsert);

            await _context.Settings.AddAsync(createSetting);
            await _context.SaveChangesAsync();

            return createSetting;
        }

        public async Task DeleteSetting(int settingId)
        {
            var settingToDelete = await _context.Settings.FindAsync(settingId);
            _context.Remove(settingToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Setting>> GetAllSettings()
        {
            var setting = await _context.Settings.ToListAsync();
            return setting;
        }

        public async Task<Setting> GetSettingById(int settingId)
        {
            var setting = await _context.Settings.FindAsync(settingId);
            if (setting == null) throw new KeyNotFoundException("Setting not found!");
            return setting;
        }

        public async Task<Setting> UpdateSetting(int settingId, SettingUpsertDtos settingUpsert)
        {
            var existingSetting = await _context.Settings.FindAsync(settingId);

            _mapper.Map(settingUpsert, existingSetting);

            await _context.SaveChangesAsync();

            return existingSetting;
        }
    }
}
