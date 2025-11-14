using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class FridgeRequestItemViewModel
    {
        // The Fridge model selected by the user
        [Required(ErrorMessage = "Please select a fridge model.")]
        [Display(Name = "Fridge Model")]
        public int SelectedFridgeId { get; set; }
        public IEnumerable<SelectListItem>? FridgeOptions { get; set; } // For the fridge model dropdown in each item row

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        public int InventoryId { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }
        public FridgeCategory SelectedCategory { get; set; }
       // public List<SelectListItem> CategoryOptions { get; set; }
        public string? Specification { get; set; }
        public string? ImageUrl { get; set; }
        public int MaxAvailableQuantity { get; set; }
        public bool IsSelected { get; internal set; }
        public int FridgeId { get;  set; }
    }

}
