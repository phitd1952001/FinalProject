using Backend.DbContext;
using Backend.Helpers.MatrixHelper;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class AutomationGenerateSchedule: IAutomationGenerateSchedule
    {
        public static DateTime StartDate;
        public static DateTime EndDate;
        public static int MaxScheduleDays;
        public static int NoOfTimeSlots;
        public static int NoOfSlotAllowOneStudentInDay;
        public static int[,] ConcurrencyLevel;
        public static int ConcurrencyLevelDefault;
        public static int D2; // external distance 10 - 4 
        public static int D1;
        public static HashSet<StudentCourses> StudentCourses = new HashSet<StudentCourses>();
        public static HashSet<Subject> Subjects = new HashSet<Subject>();
        public static Graph Graph;
        public static HashSet<Node> AllNodesHashSet;
        private readonly ApplicationDbContext _db;

        public AutomationGenerateSchedule(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task Schedule()
        {
            var begin = DateTime.Now;

            GetDataFromDb();

            var adj = BuildMatrix();

            PrintMatrix(ref adj);

            Console.WriteLine("Matrix build success Take " + (DateTime.Now - begin));

            var sortedGraph = Graph.SortAdjMatrix();

            Schedule(sortedGraph);

            PrintSchedule();

            StoreScheduleResult();

            Console.WriteLine($"Finish!!!! Take: {DateTime.Now - begin}");
        }

        private void GetDataFromDb()
        {
            HashSet<Subject> coursesDb;
            HashSet<Class> @class;

            coursesDb = _db.Subjects.AsNoTracking().ToHashSet();
            @class = _db.Classes.Include(_=>_.Subject).Include(_=>_.Account).AsNoTracking().ToHashSet();

            var setting = _db.Settings.First();

            StartDate = setting.StartDate;
            EndDate = setting.EndDate;
            ConcurrencyLevelDefault = setting.ConcurrencyLevelDefault;
            D1 = setting.InternalDistance;
            D2 = setting.ExternalDistance;
            NoOfTimeSlots = setting.NoOfTimeSlot;
            NoOfSlotAllowOneStudentInDay = setting.NoOfSlotAllowOneStudentInDay;
            MaxScheduleDays = (EndDate - StartDate).Days;
            ConcurrencyLevel = new int[MaxScheduleDays, NoOfTimeSlots];

            var result = _db.Schedules.ToList();
            
            _db.RemoveRange(result);
            _db.SaveChanges();
            
            foreach (var item in coursesDb)
            {
                var value = item.SubjectCode.Substring(0, 7);

                if (!Subjects.Any(x => x.SubjectCode == value))
                {
                    Subjects.Add(new Subject()
                    {
                        SubjectCode = value,
                        Name = value
                    });
                }
            }

            foreach (var item in @class)
            {
                if (item.Account.ManagementCode == null)
                    continue;

                StudentCourses.Add(new StudentCourses()
                {
                    StudentId = item.Account.ManagementCode,
                    CourseCode = item.Subject.SubjectCode.Substring(0, 7)
                });
            }
        }

        //TODO: recheck
        private void StoreScheduleResult()
        {
            List<Schedule> results = new List<Schedule>();
            int[] indexArray = new int[MaxScheduleDays * NoOfTimeSlots];

            int init = 0;
            for (int i = 0; i < MaxScheduleDays * NoOfTimeSlots; i++)
            {
                indexArray[i] = init;
                init++;
            }

            // calculated date and slot
            for (int i = 0; i < AllNodesHashSet.Count; i++)
            {
                int indexOfColor = Array.IndexOf(indexArray, AllNodesHashSet.ElementAt(i).Color);
                int numberOfDate = indexOfColor / NoOfTimeSlots;
                int numberOfSlot = indexOfColor % NoOfTimeSlots;
                Schedule result = new Schedule()
                {
                    SubjectCode = AllNodesHashSet.ElementAt(i).Id,
                    Color = AllNodesHashSet.ElementAt(i).Color,
                    Date = StartDate.AddDays(numberOfDate),
                    Slot = numberOfSlot
                };
                results.Add(result);
            }

            _db.Schedules.AddRange(results);
            _db.SaveChanges();
            
        }

        private int?[,] BuildMatrix()
        {
            Graph = new Graph();

            foreach (var course in Subjects)
            {
                Graph.CreateNode(course.SubjectCode);
            }

            AllNodesHashSet = Graph.AllNodes.ToHashSet();
            var count = 0;

            foreach (var course in Graph.AllNodes)
            {
                var set1 = StudentCourses.Where(c => c.CourseCode == course.Id).ToList();

                foreach (var courseToCompare in Graph.AllNodes)
                {
                    if (course.Id != courseToCompare.Id)
                    {
                        var weight = (from sc in set1
                                      join sctc in StudentCourses.Where(c => c.CourseCode == courseToCompare.Id)
                                          on sc.StudentId equals sctc.StudentId
                                      select sc).Count();

                        if (weight > 0)
                        {
                            course.AddArc(courseToCompare, weight);
                            Console.WriteLine($"Number of Arc: {count++}");
                        }
                    }
                }
            }

            return Graph.CreateAdjMatrix();
        }

        private void Schedule(HashSet<Node> c)
        {
            SetDefaultValueCl();

            int[,] colors = new int[MaxScheduleDays, NoOfTimeSlots];
            SetDefaultValueColor(colors);

            var noOfColoredCourses = 0;

            for (int i = 0; i < c.Count; i++)
            {
                if (noOfColoredCourses == c.Count)
                {
                    break;
                }


                if (c.ElementAt(i).Color == -1)
                {
                    int? rab;
                    if (i == 0)
                    {
                        rab = GetFirstNodeColor(c.ElementAt(i), colors);
                        if (rab == null)
                        {
                            break;
                        }
                    }
                    else
                    {
                        rab = GetSmallestAvailableColor(c.ElementAt(i), colors);
                    }

                    if (rab != null)
                    {
                        c.ElementAt(i).Color = (int)rab;
                        noOfColoredCourses++;
                        Console.WriteLine($"Process loading:{(double)noOfColoredCourses / c.Count}");
                        int a = -1;
                        int b = -1;
                        for (int e = 0; e < MaxScheduleDays; e++)
                        {
                            for (int f = 0; f < NoOfTimeSlots; f++)
                            {
                                if (colors[e, f] == rab)
                                {
                                    a = e;
                                    b = f;
                                    break;
                                }
                            }
                        }

                        ConcurrencyLevel[a, b] -= c.ElementAt(i).CL;
                    }

                }

                var m = GetOrderedAdjacencyCourseOf(c.ElementAt(i));
                for (int j = 0; j < m.Count; j++)
                {
                    int? rcd;
                    if (m.ElementAt(j).Color == -1)
                    {
                        rcd = GetSmallestAvailableColor(m.ElementAt(j), colors);
                        if (rcd != null)
                        {
                            m.ElementAt(j).Color = (int)rcd;
                            noOfColoredCourses++;
                            Console.WriteLine($"Process loading:{(double)noOfColoredCourses / c.Count}");
                            int a = -1;
                            int b = -1;
                            for (int e = 0; e < MaxScheduleDays; e++)
                            {
                                for (int f = 0; f < NoOfTimeSlots; f++)
                                {
                                    if (colors[e, f] == rcd)
                                    {
                                        a = e;
                                        b = f;
                                        break;
                                    }
                                }
                            }

                            ConcurrencyLevel[a, b] -= m.ElementAt(j).CL;
                        }
                    }
                }

            }
        }

        private HashSet<Node> GetOrderedAdjacencyCourseOf(Node c)
        {
            List<Node> listNodeNotSorted = new List<Node>();
            List<Node> listNodeSorted = new List<Node>();

            // copy list node
            for (int i = 0; i < c.Arcs.Count; i++)
            {
                listNodeNotSorted.Add(c.Arcs.ElementAt(i).Child);
            }

            listNodeSorted.AddRange(listNodeNotSorted.OrderByDescending(x => x.Arcs.Count)
                .ThenByDescending(x => x.MaxWeight())
                .ThenBy(x => x.Id));

            return listNodeSorted.ToHashSet();
        }

        private void SetDefaultValueCl()
        {
            for (int i = 0; i < MaxScheduleDays; i++)
            {
                for (int j = 0; j < NoOfTimeSlots; j++)
                {
                    ConcurrencyLevel[i, j] = ConcurrencyLevelDefault;
                }
            }
        }

        private void SetDefaultValueColor(int[,] colors)
        {
            int init = 0;
            for (int i = 0; i < MaxScheduleDays; i++)
            {
                for (int j = 0; j < NoOfTimeSlots; j++)
                {
                    colors[i, j] = init;
                    init++;
                }
            }
        }

        private int? GetFirstNodeColor(Node c, int[,] colors)
        {
            for (int j = 0; j < MaxScheduleDays; j++)
            {
                for (int k = 0; k < NoOfTimeSlots; k++)
                {
                    if (ConcurrencyLevel[j, k] > c.CL)
                    {
                        return colors[j, k];
                    }
                }
            }
            return null;
        }

        private int? GetSmallestAvailableColor(Node c, int[,] colors)
        {
            var alArc = c.Arcs;
            for (int j = 0; j < MaxScheduleDays; j++)
            {
                for (int k = 0; k < NoOfTimeSlots; k++)
                {
                    var valid = true;
                    for (int r = 0; r < alArc.Count; r++)
                    {                     
                        var @ref = alArc.ElementAt(r).Child.Color;
                        if (@ref != -1)
                        {
                            int a = -1;
                            int b = -1;
                            for (int e = 0; e < MaxScheduleDays; e++)
                            {
                                for (int f = 0; f < NoOfTimeSlots; f++)
                                {
                                    if (colors[e, f] == @ref)
                                    {
                                        a = e;
                                        b = f;
                                        break;
                                    }
                                }
                            }

                            if (a != j || b != k)
                            {
                                var subject = Subjects.FirstOrDefault(_=>_.SubjectCode == alArc.ElementAt(r).Child.Id);
                                var internalDistance = D1;
                                var externalDistance = D2;
                                
                                if (subject != null)
                                {
                                    var numberOfTimePrepare = subject.Credit * 0.6;
                                    externalDistance = (int)Math.Ceiling(numberOfTimePrepare);
                                    if (externalDistance < 1)
                                    {
                                        internalDistance = 2;
                                    }
                                }

                                // external distance between 2 color
                                if (Math.Abs(j - a) == externalDistance)
                                {
                                    // Internal distance between 2 color
                                    if (Math.Abs(k - b) <= internalDistance)
                                    {
                                        valid = false;
                                        break;
                                    }
                                }

                                if (ConcurrencyLevel[j, k] <= c.CL)
                                {
                                    valid = false;
                                    break;
                                }

                                if (CheckExamsConstraint(c, j, colors) == false)
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            else
                            {
                                valid = false;
                                break;
                            }
                        }
                    }

                    if (valid)
                    {
                        return colors[j, k];
                    }
                }
            }
            return null;
        }

        private bool CheckExamsConstraint(Node c, int j, int[,] colors)
        {
            var studentRegisted = StudentCourses.Where(sc => sc.CourseCode == c.Id);
            foreach (var studentRegis in studentRegisted)
            {
                var counter = 0;
                for (int q = 0; q < NoOfTimeSlots; q++)
                {
                    var crs = AllNodesHashSet.Where(n => n.Color == colors[j, q]);
                    foreach (var node in crs)
                    {
                        var exist = StudentCourses.Any(s =>
                            s.CourseCode == node.Id && s.StudentId == studentRegis.StudentId);
                        if (exist)
                        {
                            counter++;
                            // number of slot allow in 1 day
                            if (counter == NoOfSlotAllowOneStudentInDay)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void PrintMatrix(ref int?[,] matrix)
        {
            Console.Write("       ");
            for (int i = 0; i < Graph.AllNodes.Count(); i++)
            {
                Console.Write("{0}  ", Graph.AllNodes.ElementAt(i).Id);
            }

            Console.WriteLine();

            for (int i = 0; i < Graph.AllNodes.Count(); i++)
            {
                Console.Write("{0} | [ ", Graph.AllNodes.ElementAt(i).Id);

                for (int j = 0; j < Graph.AllNodes.Count(); j++)
                {
                    if (i == j)
                    {
                        Console.Write(" &,");
                    }
                    else if (matrix[i, j] == null)
                    {
                        Console.Write(" .,");
                    }
                    else
                    {
                        Console.Write(" {0},", matrix[i, j]);
                    }

                }
                Console.Write(" ]\r\n");
            }
            Console.Write("\r\n");
        }

        private void PrintSchedule()
        {
            for (int i = 0; i < AllNodesHashSet.Count; i++)
            {
                Console.WriteLine($"Course: {AllNodesHashSet.ElementAt(i).Id} - And Color: {AllNodesHashSet.ElementAt(i).Color}");
            }
        }
    }
}
