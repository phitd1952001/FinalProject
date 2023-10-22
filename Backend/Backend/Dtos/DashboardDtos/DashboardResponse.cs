namespace Backend.Dtos.DashboardDtos
{
    public class DashboardResponse
    {
        public int Users { get; set; }
        public int Slots { get; set; }
        public int Subjects { get; set; }
        public int Rooms { get; set; } 

        public List<string> Labels { get; set; }
        public List<int> TotalSlotInDay { get; set; }
        public List<int> TotalCheckInInDay { get; set; }
        public List<int> TotalRejectedInDay { get; set; }
    }
}
