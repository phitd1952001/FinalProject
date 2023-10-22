using Backend.Dtos.DashboardDtos;

namespace Backend.Services.IServices
{
    public interface IDashBoardService
    {
        Task<DashboardResponse> GetDashboard();
    }
}
