using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models
{
    public class CustomerFridge
    {
        [Key]
        public int CustomerFridgeId { get; set; }

      
        public string? SerialNumber { get; set; } // The actual serial number of *this* physical fridge
        public DateTime DatePickedUp { get; set; }

        // FK to Fridge (the model of the fridge)
        [ForeignKey("FridgeId")]
        public int FridgeId { get; set; }
        public virtual Fridge Fridge { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public int? WarehouseId { get; set; } // optional FK for warehouse pickup
        public Warehouse Warehouse { get; set; }

        //
        [ForeignKey("CustomerLocationId")]
        public int CustomerLocationId { get; set; }
        public virtual CustomerLocation CustomerLocation { get; set; }
        public virtual ICollection<FridgeFault> FridgeFaults { get; set; } = new List<FridgeFault>(); // Faults associated with this specific fridge

        public virtual ICollection<FaultReport> FaultReports { get; set; } = new List<FaultReport>(); // Faults associated with this specific fridge
    }
}
