using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models
{
    public class FaultTechnician
    {
        [Key]
        public int FaultTechId { get; set; } // Changed from TechnicianId
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime? LastFaultAssignedDate { get; set; }
        public int? CurrentActiveFaultCount { get; set; }

        // NEW: FK to the Warehouse this technician is associated with
        public int WarehouseId { get; set; }
        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; }

        // Link to ApplicationUser (if fault technicians also log in)
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<AssignedFault> AssignedFaults { get; set; } = new List<AssignedFault>();
    }
}
