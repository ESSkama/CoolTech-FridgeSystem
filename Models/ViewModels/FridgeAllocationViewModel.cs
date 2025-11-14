using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models.ViewModels
{
    public class FridgeAllocationViewModel
    {
        public int AllocationId { get; set; }

        [Required(ErrorMessage = "Please select a customer.")]
        [Display(Name = "Customer")]
        public int BusinessId { get; set; }

        [Required(ErrorMessage = "Please select a location.")]
        [Display(Name = "Location")]
        public int LocationId { get; set; }

        [Display(Name = "Fridge")]
        public int FridgeId { get; set; } // Used only in Edit

        // Read-only display fields
        public string? BusinessName { get; set; }
        public string? BranchName { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }

        public int AllocatedById { get; set; }
        public DateTime AllocatedDate { get; set; }

        // Dropdowns
        public IEnumerable<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Locations { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Fridges { get; set; } = new List<SelectListItem>();

        // Requested fridges
        [Required(ErrorMessage = "Please select at least one fridge.")]
        [Display(Name = "Requested Fridges")]
        public List<FridgeRequestItemViewModel> RequestedFridges { get; set; } = new List<FridgeRequestItemViewModel>();
    }
}
