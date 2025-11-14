using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models.ViewModels
{
    public class PurchaseRequestViewModel
    {
        public int PurchaseRequestId { get; set; }

        [Required]
        [Display(Name = "Fridge Model")]
        public int? InventoryId { get; set; }

        // FIX 1: Make FridgeModel nullable since it's only for display.
        public string? FridgeModel { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        // FIX 2: Make RequestedByName nullable since it's only for display.
        [Display(Name = "Requested By")]
        public string? RequestedByName { get; set; }

        [Display(Name = "Requested Date")]
        public DateTime? RequestedDate { get; set; }

        [Required(ErrorMessage = "A reason for the request is required.")]
        [Display(Name = "Reason for Request")]
        public string ReasonForRequest { get; set; }

        public RequestStatus Status { get; set; }
        public List<SelectListItem> FridgeModels { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
