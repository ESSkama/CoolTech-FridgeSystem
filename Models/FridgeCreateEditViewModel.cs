using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class FridgeCreateEditViewModel
    {
        public int FridgeId { get; set; } // Will be 0 for Create, >0 for Edit


        public string? Model { get; set; }

        public string? SerialNumber { get; set; }

        public string Specification { get; set; }

        [Required(ErrorMessage = "Fridge Color is required.")]
        public string FridgeColor { get; set; }

        public IFormFile? FridgeImageFile { get; set; }

        [Display(Name = "Image URL")]
        [DataType(DataType.Url, ErrorMessage = "Please enter a valid URL.")]
        public string? FridgeImage { get; set; }

        [Required(ErrorMessage = "Stock Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity must be a non-negative number.")]
        public int StockQuantity { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; } = true; // Default to true for new fridges

        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        public DateTime? PurchaseDate { get; set; }

        [Display(Name = "Warranty Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime? WarrantyExpiryDate { get; set; }

        [Display(Name = "Delivery Date")]
        [DataType(DataType.Date)]
        public DateTime? DeliveryDate { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Category is required.")]
        public FridgeCategory Category { get; set; } // FridgeCategory enum

        // For Supplier Dropdown
        [Display(Name = "Supplier")]
        [Required(ErrorMessage = "Supplier is required.")]
        public int? SelectedSupplierId { get; set; }
        public IEnumerable<SelectListItem>? Suppliers { get; set; }


        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "Please select a warehouse")]
        public int? WarehouseId { get; set; }
        public IEnumerable<SelectListItem>? Warehouses { get; set; }

        // Inventory Model Linking

       // [Display(Name = "Inventory Model")]
        //[Required(ErrorMessage = "Parent Inventory Model is required.")]
        // public int? InventoryId { get; set; }
        // public IEnumerable<SelectListItem>? InventoryModels { get; set; }
    
    }
}
