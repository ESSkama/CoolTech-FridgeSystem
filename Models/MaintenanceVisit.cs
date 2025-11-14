namespace FridgeSystem.Models
{
    public class MaintenanceVisit
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = "Scheduled";

        public string TechnicianId { get; set; }
        public ApplicationUser Technician { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int FridgeId { get; set; }
        public Fridge Fridge { get; set; }

        public string Notes { get; set; }
    }
}