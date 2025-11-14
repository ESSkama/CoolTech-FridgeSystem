using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models.ViewModels
{
    public class FridgeUnitViewModel
    {
        public int FridgeId { get; set; }
                
        [Required(ErrorMessage = "Model is required")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Serial number is required")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Color is required")]
        public string FridgeColor { get; set; }

        public string FridgeDescription { get; set; }

        // Stores the image path
        public string FridgeImage { get; set; }

        // For uploading a new image
        public IFormFile FridgeImageFile { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        [Required(ErrorMessage = "Please select a fridge category")]
        public FridgeCategory? Category { get; set; }

        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "Please select a warehouse")]
        public int? WarehouseId { get; set; }

        [Display(Name = "Inventory")]
        [Required(ErrorMessage = "Please select a fridge model")]
        public int? InventoryId { get; set; }

        [Display(Name = "Supplier")]
        [Required(ErrorMessage = "Please select a supplier")]
        public int SelectedSupplierId { get; set; }

        public string SupplierName { get; set; }
        public string BranchName { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool? isDeleted { get; set; } = false;
        public string Status { get; set; }

        public int StockQuantity { get; set; }
        public List<string> WarehouseNames { get; set; } = new();

        public IEnumerable<SelectListItem> FridgeModels { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Warehouses { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>();

        public string FridgeHistoryAction { get; set; }
        public string ScrapFridgeAction { get; set; }
        public string EditFridgeUnitAction { get; set; }
        public string ViewFridgeDetailsAction { get; set; }
    }
}