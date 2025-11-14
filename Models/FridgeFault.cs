using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class FridgeFault
    {
        [Key]

        public int FridgeFaultId { get; set; }

        // FK to the specific customer fridge
        public int CustomerFridgeId { get; set; }
        public virtual CustomerFridge CustomerFridge { get; set; }
        public string Title { get; set; }
        public FaultStatus FaultStatus { get; set; } 
        public FaultCategory SelectedCategory { get; set; } 
        public string FaultDescription { get; set; }
        public DateTime LoggedDate { get; set; } = DateTime.Now;
        public FaultStatus Status { get; set; } 
        public string? ResolutionNotes { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public int? TechnicianId { get; set; }
        public Employee? Technician { get; set; }
        public virtual ICollection<AssignedFault> AssignedFaults { get; set; }
        public ICollection<ServiceSchedule> ServiceSchedules { get; set; }

    }
}

