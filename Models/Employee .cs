using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FridgeSystem.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(10)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        [Required]
        public string Province { get; set; } = null!; // already seeded for dropdown

        [Required]
        public DateTime HiredDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

       
        public string ApplicationUserId { get; set; } = null!;

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

      
        [Display(Name = "Warehouse")]
        public int? WarehouseId { get; set; } // Nullable in case not assigned yet

        [ForeignKey("WarehouseId")]
        public virtual Warehouse? Warehouse { get; set; }

       
        // Relationships

        public ICollection<FridgeInventory> ManagedInventories { get; set; } = new List<FridgeInventory>();
        public ICollection<FridgeAllocation> Allocations { get; set; } = new List<FridgeAllocation>();
        public ICollection<AssignedFault> AssignedFaults { get; set; } = new List<AssignedFault>();
        public ICollection<FaultReport> AllocatedFaults { get; set; } = new List<FaultReport>();
        public ICollection<FaultDiagnostic> Diagnostics { get; set; } = new List<FaultDiagnostic>();
        public ICollection<ServiceSchedule> ServiceSchedules { get; set; } = new List<ServiceSchedule>();
        public ICollection<ServiceAssignment> ServicesAssigned { get; set; } = new List<ServiceAssignment>();
        public ICollection<ServiceAssignment> AssignedServices { get; set; } = new List<ServiceAssignment>();
        public ICollection<ServiceChecklist> ServiceChecklists { get; set; } = new List<ServiceChecklist>();
        public ICollection<ServiceCheckResult> ServiceCheckresults { get; set; } = new List<ServiceCheckResult>();
        public ICollection<PurchaseRequest> PurchaseRequestsSubmitted { get; set; } = new List<PurchaseRequest>();
        public ICollection<PurchaseRequest> PurchaseRequests { get; set; } = new List<PurchaseRequest>();
        public ICollection<FridgeRequest> FridgeRequests { get; set; } = new List<FridgeRequest>();
        public ICollection<FridgeStatusHistory> FridgeStatusHistory { get; set; } = new List<FridgeStatusHistory>();
        public ICollection<EmployeeNotification> Notifications { get; set; } = new List<EmployeeNotification>();
    }
}
