using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class ServiceSchedule
    {
        [Key]
        public int ScheduleId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // can be null so date can be added later
        public FaultStatus Status { get; set; } 
        public int FridgeId { get; set; }
        public Fridge Fridge { get; set; }
        //fk to Customer Location
        public int LocationId { get; set; }
        public CustomerLocation CustomerLocation { get; set; }
        //fk to fault report table
        public int? FaultId { get; set; }
        public FaultReport? FaultReport { get; set; }
        //fk to event type table
        public int? EventId { get; set; }
        public EventType? ServiceType { get; set; }
        //fk to employee 
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int FridgeFaultId { get; set; }
        public FridgeFault FridgeFault { get; set; }

        public ICollection<FaultDiagnostic> FaultDiagnostics { get; set; } = new List<FaultDiagnostic>();
        public ICollection<ServiceAssignment> ServiceAssignments { get; set; } = new List<ServiceAssignment>();
        public ICollection<ServiceCheckResult> ServiceCheckResults { get; set; } = new List<ServiceCheckResult>();
        public ICollection<CustomerNotification> Notifications { get; set; } = new List<CustomerNotification>();
    }
}
