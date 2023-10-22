using AutoMapper;
using Backend.DbContext;
using Backend.Dtos.DashboardDtos;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class DashboardService: IDashBoardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardResponse> GetDashboard()
        {
            var result = new DashboardResponse
            {
                Users = await _context.Accounts.CountAsync(),
                Rooms = await _context.Rooms.CountAsync(),
                Slots = await _context.Slots.CountAsync(),
                Subjects = await _context.Subjects.CountAsync(),
            };

            var groupSlotsByDate = _context.Slots
                .GroupBy(item => item.StartTime.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    Count = group.Count(),
                })
                .ToList();

            result.Labels = groupSlotsByDate.Select(item => item.Date.ToString("MM/dd/yyyy")).ToList();
            result.TotalSlotInDay = groupSlotsByDate.Select(item => item.Count).ToList();

            var allSlots = await _context.Slots.ToListAsync();
            var slotIdsByDate = allSlots
                .GroupBy(item => item.StartTime.Date)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(slot => slot.Id).ToList()
                );

            result.TotalCheckInInDay = groupSlotsByDate
                .Select(item => slotIdsByDate.ContainsKey(item.Date)
                    ? _context.Checkins.Count(checkin => slotIdsByDate[item.Date].Contains(checkin.SlotId))
                    : 0)
                .ToList();

            result.TotalRejectedInDay = groupSlotsByDate
                .Select(item => slotIdsByDate.ContainsKey(item.Date)
                    ? _context.Checkins.Count(checkin => slotIdsByDate[item.Date].Contains(checkin.SlotId) && !checkin.IsAccept)
                    : 0)
                .ToList();

            return result;

        }
    }
}
