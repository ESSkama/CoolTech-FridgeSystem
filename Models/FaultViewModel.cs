using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models
{
    public class FaultViewModel
    {
       
        public int RequestId { get; set; }   // From approved request
        public FridgeRequest FridgeRequest { get; set; }
        public int BusinessId { get; set; }  // Logged-in customer
        public Customer Business { get; set; }
        public int CustomerLocationId { get; set; }  // Optional
        public CustomerLocation CustomerLocation { get; set; }
      
        [Required]
        [Display(Name = "Select Fridge")]
        public int FridgeId { get; set; }
        public List<SelectListItem>? Fridges { get; set; }

        // Fault information
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        [Display(Name = "Fault Description")]
        public string Description { get; set; }
        public DateTime LoggedDate { get; set; } = DateTime.Now;
    }
}
