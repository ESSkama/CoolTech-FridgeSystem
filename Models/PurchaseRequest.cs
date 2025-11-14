using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class PurchaseRequest
    {
        [Key]
        public int RequestId { get; set; }
        [Required]
        public DateTime RequestedDate { get; set; }
        [Required]
        public int QuantityRequested { get; set; }

        public string ReasonForRequest { get; set; }



        //fk to employee who makes the request 
        public int RequestedById { get; set; }  // FK
        public Employee RequestedBy { get; set; }  // Navigation property
        //fk to manager
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int InventoryId { get; set; }
        public FridgeInventory Inventory { get; set; }

        public RequestStatus RequestStatus { get; set; }

    }
}
