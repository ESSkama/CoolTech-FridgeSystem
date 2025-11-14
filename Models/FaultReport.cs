using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class FaultReport
    {
        [Key]
        public int FaultId { get; set; }
       
        //public string  Title { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public string PriorityLevel { get; set; }// set by admin when reviewing the report 
        [Required]
        public DateTime ReportedDate { get; set; } = DateTime.Now;
        public FaultCategory Title { get; set; }

        //fk to customer who reported fault
        public int BusinessId { get; set; }
        public Customer Customer { get; set; }
        //
        public int CustomerFridgeId { get; set; }
        public CustomerFridge CustomerFridge { get; set; }

        public int InventoryId { get; set; }
        public FridgeInventory FridgeInventory { get; set; }
        //fk to which location the fault is at
        public int LocationId { get; set; }
        public CustomerLocation Location { get; set; }
        //fk to the admin who assigned the fault
        public int? AssignedById { get; set; }//can be assigned once reviewed 
        public Employee Admin { get; set; }
        //fk to the techinicain assigned to the fault
        public int? AssignedToId { get; set; }
        public Employee FaultTechnician { get; set; }
        public int FridgeFaultId { get; set; }    // Foreign Key
        public FridgeFault FridgeFault { get; set; } // Navigation Property
        public RequestStatus RequestStatus { get; set; }
        public ICollection<AssignedFault> AssignedFaults { get; set; }

        public ICollection<FaultDiagnostic> Diagnostics { get; set; } = new List<FaultDiagnostic>();
        public ICollection<ServiceSchedule> Schedules { get; set; } = new List<ServiceSchedule>();
        public ICollection<CustomerNotification> Notifications { get; set; } = new List<CustomerNotification>();
        public ICollection<ServiceAssignment> ServiceAssignments { get; set; } = new List<ServiceAssignment>();
        public ICollection<Fridge> Fridges { get; set; } = new List<Fridge>();
        
    }
}
