using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models
{
    public class AssignedFault
    {
        [Key]
        public int AssignedFaultId { get; set; }

        // FK to FridgeFault
        public int FridgeFaultId { get; set; }
        public virtual FridgeFault FridgeFault { get; set; }

        // NEW: Updated FK name
        public int? FaultReportId { get; set; }  // Foreign Key
        public FaultReport? FaultReport { get; set; }
        public int? TechnicianId { get; set; }
        [ForeignKey("FaultTechId")]
        public virtual Employee? Technician { get; set; }
        public string? FridgeImage { get; set; }

        public DateTime AssignmentDate { get; set; } = DateTime.Now;
        public DateTime? CompletionDate { get; set; }
        public string? AdminNotes { get; set; }
    }
}
