namespace FridgeSystem.Models.ViewModels
{
    public class StockAlertViewModel
    {
        public int InventoryId { get; set; }
        public string Model { get; set; }
        public string Category { get; set; }
        public int QuantityInStock { get; set; }
        public string SupplierName { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}