using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models
{
    public class FridgeRequest
    {
        [Key]
        public int RequestId { get; set; }
       
       // public int Quantity { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string ContactPerson { get; set; } // Contact for *this specific request* (could be different from business main contact)

        public string ContactNumber { get; set; } // Contact for *this specific request* (could be different from business main contact)
        [MaxLength(500)]
        public string? SpecialInstructions { get; set; }
        public int Quantity { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime PreferredPickup { get; set; }

        public DateTime ApprovedDate { get; set; } = DateTime.Now;
         
        // New property to link to Warehouse
        public int WarehouseId { get; set; }
        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; } // Navigation property

        public int CustomerLocationId { get; set; }
        public CustomerLocation CustomerLocation { get; set; }
        [ForeignKey("CustomerId")]
        public string CustomerId { get; set; }
        public virtual ApplicationUser Customer { get; set; }  // Navigation property
        public int BusinessId { get; set; }
        [ForeignKey("BusinessId")]
        public virtual Customer Business { get; set; }
       
        public int? ReviewedById { get; set; }
        public Employee ReviewedBy { get; set; }

        
        public ICollection<FridgeAllocation> FridgeAllocations { get; set; } = new List<FridgeAllocation>();

        // Navigation property for 1:M relationship with FridgeRequestItem
        public virtual ICollection<FridgeRequestItem> FridgeRequestItems { get; set; } = new List<FridgeRequestItem>();
    }
}
