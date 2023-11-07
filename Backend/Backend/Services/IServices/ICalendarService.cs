namespace Backend.Services.IServices
{
    public interface ICalendarService
    {
        Task<object> GetAllScheduler();
        Task<object> GetSchedulerByUserId(int userId);
    }
}
