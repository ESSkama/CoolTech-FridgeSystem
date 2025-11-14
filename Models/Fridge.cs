using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models
{
    public class Fridge
    {
        public int FridgeId { get; set; }
       
        public string Model { get; set; }
        public string? SerialNumber { get; set; }
        public string Specification { get; set; }       
        public string FridgeColor { get; set; }      
        public string? FridgeImage { get; set; }
        public int StockQuantity { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? isDeleted { get; set; }     
        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public FridgeCategory Category{ get; set; }

       // public int WarehouseId { get; set; }
        //public  Warehouse Warehouse { get; set; }

        public virtual ICollection<FridgeRequestItem> FridgeRequestItems { get; set; } = new List<FridgeRequestItem>();

        public int? SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }
         public int? WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; }
       public virtual ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();

        public int? CustomerLocationId { get; set; }
        public CustomerLocation CustomerLocation { get; set; }

        public int? InventoryId { get; set; }
        public FridgeInventory Inventory { get; set; }
        public ICollection<CustomerFridge>? CustomerFridges { get; set; } //= new List<CustomerFridge>();   

        public ICollection<FridgeAllocation>? FridgeAllocations { get; set; } //= new List<FridgeAllocation>();    

        public ICollection<ServiceSchedule> ServiceSchedules { get; set; } = new List<ServiceSchedule>();
        public ICollection<FridgeStatusHistory> FridgeStatusHistory { get; set; } = new List<FridgeStatusHistory>();

    }
}
