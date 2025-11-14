using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class FridgeRequestViewModel
    {
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }

        [MaxLength(500, ErrorMessage = "Special instructions cannot exceed 500 characters.")]
        public string? SpecialInstructions { get; set; }

        [Required(ErrorMessage = "Preferred pickup date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Preferred Pickup Date")]
        public DateTime PreferredPickup { get; set; }
        public string Model { get; set; }

        // For Warehouse selection (FK from FridgeRequest)
        [Required(ErrorMessage = "Pickup location is required.")]
        [Display(Name = "Pickup Warehouse")]
        public int SelectedWarehouseId { get; set; }
        public IEnumerable<SelectListItem>? WarehouseOptions { get; set; } // For the dropdown


        [Required(ErrorMessage = "Please select your branch.")]
        public int SelectedBranchId { get; set; }
        public List<SelectListItem> BranchOptions { get; set; }


        // NEW: Fridge Category selection
        public FridgeCategory SelectedCategory { get; set; }
        public List<SelectListItem> CategoryOptions { get; set; }




        public List<AvailableFridgeDisplayModel> AllAvailableFridges { get; set; } = new List<AvailableFridgeDisplayModel>();

        // For handling multiple fridge selections and quantities
        public List<FridgeRequestItemViewModel> RequestedFridges { get; set; } = new List<FridgeRequestItemViewModel>();

        public int RequestId { get; set; }
        public int BusinessId { get; set; }
        public int FridgeId { get; set; }
        [Display(Name = "Customer")]
        public string? BusinessName { get; set; }

        public int CustomerLocationId { get; set; }

        [Display(Name = "Branch")]
        public string? BranchName { get; set; }
        public List<CartItem>? CartItems { get; set; } = new();
    }
}
