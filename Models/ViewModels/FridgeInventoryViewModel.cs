using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models.ViewModels
{
    public class FridgeInventoryViewModel
    {
        public int InventoryId { get; set; }

        [Required(ErrorMessage = "The Fridge Model field is required.")]
        public string FridgeModel { get; set; }

        [Required(ErrorMessage = "The Category field is required.")]
        public string FridgeCategory { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or greater.")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage = "The Supplier field is required.")]
        public int? SupplierId { get; set; }

        public IEnumerable<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>();

        public int? SelectedWarehouseId { get; set; }
        public IEnumerable<SelectListItem> WarehousesDropdown { get; set; } = new List<SelectListItem>();

        // **CRITICAL FIX 1:** Make SupplierName nullable (string?)
        // This resolves: "The SupplierName field is required."
        public string? SupplierName { get; set; }

        // **CRITICAL FIX 2:** Make WarehousesNames nullable (string?)
        // This resolves: "The WarehousesNames field is required."
        public string? WarehousesNames { get; set; }
    }
}
