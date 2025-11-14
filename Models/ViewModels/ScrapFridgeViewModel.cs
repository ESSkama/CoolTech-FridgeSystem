using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models.ViewModels
{
    public class ScrapFridgeViewModel
    {
        public int FridgeId { get; set; }
        public string Model { get; set; }
        public int QuantityInStock { get; set; }

        [Required]
        [Display(Name = "Reason for Scrapping")]
        public string Reason { get; set; }

        public string WarehouseName { get; set; }
    }
}
