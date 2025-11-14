namespace FridgeSystem.Models.ViewModels
{
    public class FridgeDetailsViewModel
    {
        public int FridgeId { get; set; }
        public string FridgeName { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string FridgeColor { get; set; }
        public string FridgeDescription { get; set; }
        public string FridgeImage { get; set; }

        public string WarehouseName { get; set; } 
        public string SupplierName { get; set; }  
        public string Category { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
