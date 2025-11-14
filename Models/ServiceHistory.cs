namespace FridgeSystem.Models
{
    public class ServiceHistory
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }

        public int FridgeId { get; set; }
        public Fridge Fridge { get; set; }

        public string TechnicianId { get; set; }
        public ApplicationUser Technician { get; set; }
    }
}
