using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models
{
    public class FridgeInventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public int QuantityInStock { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }

        // 1 fridge model, many fridges
        public ICollection<Fridge> Fridges { get; set; }

        // Supplier FK
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        // FK to Inventory Liaison
        [ForeignKey("InventoryLiason")]
        public int InventoryLiasonEmployeeId { get; set; } // must match DB column name
        public Employee InventoryLiason { get; set; }

        // Other collections
        public ICollection<PurchaseRequest> PurchaseRequests { get; set; } = new List<PurchaseRequest>();
        public ICollection<FridgeRequest> FridgeRequests { get; set; } = new List<FridgeRequest>();
        public ICollection<FridgeAllocation> FridgeAllocations { get; set; } = new List<FridgeAllocation>();
        public ICollection<FaultReport> FaultReports { get; set; } = new List<FaultReport>();
        public ICollection<FridgeStatusHistory> FridgeStatusHistory { get; set; } = new List<FridgeStatusHistory>();
        public virtual ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
    }
}
