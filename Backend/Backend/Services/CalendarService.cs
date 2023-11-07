using Backend.DbContext;
using Backend.Helpers.MatrixHelper;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class CalendarService : ICalendarService
    {
        private ApplicationDbContext _db;

        public CalendarService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<object> GetAllScheduler()
        {
            var schedulers = await _db.Schedules.ToListAsync();
            var schedulerRoomMapped = await _db.ScheduleRoomMaps.ToListAsync();
            var subjects = await _db.Subjects.ToListAsync();
            var rooms = await _db.Rooms.ToListAsync();
            var students = await _db.Accounts.Where(_=>_.Role == Models.Role.Student).ToListAsync();

            var data = new List<object>();
     
            foreach (var item in schedulers)
            {
                var subject = subjects.FirstOrDefault(_ => _.SubjectCode == item.SubjectCode);
                if (subject == null)
                    continue;

                var schedulerRoomMap = schedulerRoomMapped.Where(_ => _.SchedulerId == item.Id);
                if (schedulerRoomMap == null)
                    continue;

                string des = "";

                var numberOfRoomForOneSubject = schedulerRoomMap.Select(_ => _.RoomId);
                foreach (var roomId in numberOfRoomForOneSubject)
                {
                    var room = rooms.FirstOrDefault(_ => _.Id == roomId);
                    if (room == null)
                        continue;

                    var schedulerRoomMapForEachRoom = schedulerRoomMap.FirstOrDefault(_ => _.RoomId == roomId);

                    var studentIds = schedulerRoomMapForEachRoom.StudentIds.Split(",").Select(_=>Convert.ToInt32(_)).ToList();

                    var studentManagementCodes = students.Where(_ => studentIds.Contains(_.Id)).Select(_=>_.ManagementCode).Distinct().ToList();

                    des += room.Name + ":\n";
                    foreach (var studentCode in studentManagementCodes)
                    {
                        des += "\t" + studentCode + "\n";
                    }
                }

                var startDate = MatrixCalculator.CalculateStartTime(item.Date, item.Slot);
                data.Add(new
                {
                    text = subject.SubjectCode,
                    startDate = startDate,
                    endDate = startDate.AddMinutes(subject.Duration),
                    description = des
                });
                
            }

            return data;
        }


        public async Task<object> GetSchedulerByUserId(int userId)
        {
            var schedulers = await _db.Schedules.ToListAsync();
            var schedulerRoomMapped = await _db.ScheduleRoomMaps.ToListAsync();
            var subjects = await _db.Subjects.ToListAsync();
            var rooms = await _db.Rooms.ToListAsync();
            var students = await _db.Accounts.Where(_ => _.Role == Models.Role.Student).ToListAsync();

            var data = new List<object>();

            foreach (var item in schedulers)
            {
                var subject = subjects.FirstOrDefault(_ => _.SubjectCode == item.SubjectCode);
                if (subject == null)
                    continue;

                var schedulerRoomMap = schedulerRoomMapped.Where(_ => _.SchedulerId == item.Id);
                if (schedulerRoomMap == null)
                    continue;

                string des = "";

                var numberOfRoomForOneSubject = schedulerRoomMap.Select(_ => _.RoomId);
                var isAllAdded = false;
                foreach (var roomId in numberOfRoomForOneSubject)
                {
                    var room = rooms.FirstOrDefault(_ => _.Id == roomId);
                    if (room == null)
                        continue;

                    var schedulerRoomMapForEachRoom = schedulerRoomMap.FirstOrDefault(_ => _.RoomId == roomId);

                    var studentIds = schedulerRoomMapForEachRoom.StudentIds.Split(",").Select(_ => Convert.ToInt32(_)).ToList();

                    if (studentIds.Contains(userId))
                    {
                        isAllAdded = true;
                    }

                    var studentManagementCodes = students.Where(_ => studentIds.Contains(_.Id)).Select(_ => _.ManagementCode).Distinct().ToList();

                    des += room.Name + ":\n";
                    foreach (var studentCode in studentManagementCodes)
                    {
                        des += "\t" + studentCode + "\n";
                    }
                }

                if (isAllAdded)
                {
                    var startDate = MatrixCalculator.CalculateStartTime(item.Date, item.Slot);
                    data.Add(new
                    {
                        text = subject.SubjectCode,
                        startDate = startDate,
                        endDate = startDate.AddMinutes(subject.Duration),
                        description = des
                    });
                }  

            }

            return data;
        }
    }
}
