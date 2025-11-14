using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class FridgeAllocation
    {
        [Key]
        public int AllocationId { get; set; }
        [Required]
        public DateTime AllocatedDate { get; set; }
        public DateTime DeliveryDate { get; set; }  //? -- to make date null for when fridge is not yet delivered
        public string? Notes { get; set; }


        //fk to fridges via inventory
        public int InventoryId { get; set; }
        public FridgeInventory FridgeInventory { get; set; }

        public int FridgeId { get; set; }
        public Fridge Fridge {  get; set; }

        public int BusinessId { get; set; }
        public Customer BusinessName { get; set; }

        //fk to locations 
        public int LocationId { get; set; }
        public CustomerLocation CustomerLocation { get; set; }
        //fk to status
       public RequestStatus RequestStatus { get; set; }
        //fk to customer liason
        public int AllocatedById { get; set; }
        public Employee CustomerLiaison { get; set; }

        public int RequestId { get; set; }
        public FridgeRequest FridgeRequest { get; set; }

    }
}
