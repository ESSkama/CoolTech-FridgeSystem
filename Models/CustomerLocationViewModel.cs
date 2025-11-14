using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class CustomerLocationViewModel
    {
       
        public int LocationId { get; set; }

        public int BusinessId { get; set; }

        [Required(ErrorMessage = "Branch name is required.")]
        [MaxLength(100, ErrorMessage = "Branch Name cannot exceed 100 characters.")]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; } = null!;

        [Required(ErrorMessage = "Address Line 1 is required.")]
        [MaxLength(100, ErrorMessage = "Address Line 1 cannot exceed 100 characters.")]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; } = null!;

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Province is required.")]
        [MaxLength(50, ErrorMessage = "Province cannot exceed 50 characters.")]
        public string Province { get; set; } = null!;

        [Required(ErrorMessage = "Postcode is required.")]
        [MaxLength(10, ErrorMessage = "Postcode cannot exceed 10 characters.")]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; } = null!;

        
        [MaxLength(100, ErrorMessage = "Address Line 2 cannot exceed 100 characters.")]
        [Display(Name = "Address Line 2")]
        public string? AddressLine2 { get; set; }

        [MaxLength(100, ErrorMessage = "Contact Person name cannot exceed 100 characters.")]
        [Display(Name = "Contact Person")]
        public string? ContactPerson { get; set; }

        [MaxLength(15, ErrorMessage = "Telephone number cannot exceed 15 characters.")]
        public string? Telephone { get; set; }

       
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public string? BusinessName { get; set; }

        public string? SearchText { get; set; }
        public string? StatusFilter { get; set; }
    }
}
