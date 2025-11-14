using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class CustomerViewModel
    {
        public int BusinessId { get; set; } // Null or 0 for Add

        [Required]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Telephone")]
        public string Telephone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Billing Address")]
        public string BillingAddress { get; set; } = string.Empty;

        // --- Business Type Fields ---
        [Required]
        [Display(Name = "Business Type")]
        public int BusinessTypeId { get; set; }  // Selected type from dropdown

        public string? BusinessTypeName { get; set; } // Display-only name
        public IEnumerable<SelectListItem>? BusinessTypeList { get; set; } // Dropdown data source

        // --- Other Customer Info ---
        [Display(Name = "Contact Person")]
        public string? ContactPersonName { get; set; }

        [Display(Name = "Active")]
        public bool ProfileActive { get; set; } = true;

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public int TotalLocations { get; set; }

        // --- Authentication (for new customer account) ---
        [DataType(DataType.Password)]
        public string? Password { get; set; } // Optional for edit

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        // --- Search / Filter Support ---
        public string? SearchText { get; set; }
        public string? StatusFilter { get; set; }
    }
}
