namespace FridgeSystem.Models
{
    public class AvailableFridgeDisplayModel
    {
        public int FridgeId { get; set; }
        public string Model { get; set; } = string.Empty;
        public FridgeCategory Category { get; set; }
        public string Specification { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty; // Crucial for displaying pictures
        public int AvailableQuantity { get; set; }
        public string SupplierName { get; set; } // Add supplier name

    }
}
