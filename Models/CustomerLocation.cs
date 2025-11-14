using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models
{
    public class CustomerLocation
    {
        [Key]

        public int CustomerLocationId { get; set; }

        // FK to Customer
        public int BusinessId { get; set; }
        [ForeignKey("BusinessId")]
        public virtual Customer Customer { get; set; }

        [Required]
        public string BranchName { get; set; } // e.g., "Main Office", "Branch A", "Warehouse 1"
        [Required]
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Province { get; set; } // Crucial for technician matching : //seed all 9 provices  for dropdown
        [Required]
        public string Postcode { get; set; }
        public string? ContactPerson { get; set; }
        public string? Telephone { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Fridge> Fridges { get; set; }
        public ICollection<FridgeAllocation> FridgeAllocations { get; set; } = new List<FridgeAllocation>();
        public ICollection<FaultReport> FaultReports { get; set; } = new List<FaultReport>();
        public ICollection<ServiceSchedule> ServiceSchedules { get; set; } = new List<ServiceSchedule>();
        public ICollection<FridgeRequest> FridgeRequests { get; set; } = new List<FridgeRequest>();
        public ICollection<FridgeStatusHistory> FridgeStatusHistory { get; set; } = new List<FridgeStatusHistory>();
        // Navigation property for fridges at this location
        public virtual ICollection<CustomerFridge> CustomerFridges { get; set; } = new List<CustomerFridge>();
    }
}
