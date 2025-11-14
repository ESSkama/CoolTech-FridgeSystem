namespace FridgeSystem.Models.ViewModels
{
    public class TechnicianDashboardViewModel
    {
        public string TechnicianName { get; set; }
        public List<FridgeFault> NewlyAssigned { get; set; }
        public List<FridgeFault> PendingDiagnosis { get; set; }
        public List<FridgeFault> ScheduledRepairs { get; set; }
        public List<FridgeFault> CompletedFaults { get; set; }
    }
}