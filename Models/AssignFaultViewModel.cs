using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class AssignFaultViewModel
    {
        public int FaultId { get; set; }

        // Warehouse selection
        [Required]
        public int SelectedWarehouseId { get; set; }
        public List<SelectListItem> Warehouses { get; set; }
        public int CurrentTechnicianId { get; set; } // For pre-selection in reassignment

        // Technician selection
        [Required]
        public int SelectedTechnicianId { get; set; }
        public List<TechnicianWithCount> Technicians { get; set; }
    }
}
