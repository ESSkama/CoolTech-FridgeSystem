namespace FridgeSystem.Models
{
    public class ScheduleServiceViewModel
    {
        public int FridgeFaultId { get; set; }
        public string Customer { get; set; }
        public string ContactPerson { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string TechnicianName { get; set; } // e.g. "John Doe"
        public int? EmployeeId { get; set; } // optional if you need the ID
    }
}
