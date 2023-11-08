using Backend.DbContext;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class ReminderService : IReminderService
    {
        private ApplicationDbContext _db;
        private IEmailService _emailService;

        public ReminderService(ApplicationDbContext db, IEmailService emailService) 
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task Run()
        {
            var now = DateTime.Now;
            var toDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 59, 999, DateTimeKind.Local);

            var eventsToSend = await _db.Reminders
                .Where(remind => remind.DateTimePerformed <= toDate)
                .OrderBy(remind => remind.DateTimePerformed)
                .ToListAsync();

            if (eventsToSend.Count == 0)
                return;

            var reminderSent = new List<Reminder>();
            foreach (var emailEvent in eventsToSend)
            {
                try
                {
                    await _emailService.Send(emailEvent.To, "Exam Reminder", emailEvent.Message);
                    reminderSent.Add(emailEvent);
                }
                catch (Exception e)
                {
                    
                }
            }

            _db.Reminders.RemoveRange(reminderSent);
            _db.SaveChanges();
        }
    }
}
