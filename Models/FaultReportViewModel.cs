using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models
{
    public class FaultReportViewModel
    {
        [Required]
        public int FridgeId { get; set; }

        [Required]
        public int BusinessId { get; set; }

        
        public string? Title { get; set; }

        [Required(ErrorMessage = "Please select a fault category")]
        public FaultCategory SelectedCategory { get; set; }

        [NotMapped]
        public List<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();

        [Required(ErrorMessage = "Please provide a description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        // Optional
        public int? CustomerLocationId { get; set; }

        public CustomerLocation? CustomerLocation { get; set; }

    }
}
