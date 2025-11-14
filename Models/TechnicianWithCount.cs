namespace FridgeSystem.Models
{
    public class TechnicianWithCount
    {
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; }
        public int AssignedFaultCount { get; set; }
    }
}
