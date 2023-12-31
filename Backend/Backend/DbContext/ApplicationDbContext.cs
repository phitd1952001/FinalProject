using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.DbContext
{
    public class ApplicationDbContext: Microsoft.EntityFrameworkCore.DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Checkin> Checkins { get; set; }
        
        public DbSet<Class> Classes { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Reminder> Reminders { get; set; }

        public DbSet<Room> Rooms { get; set; }
        
        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<ScheduleRoomMap> ScheduleRoomMaps { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
    }
}