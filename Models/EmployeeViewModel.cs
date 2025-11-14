using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class EmployeeViewModel
    {
        public int? EmployeeId { get; set; } // null for Add

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        public string PhoneNumber { get; set; } = null!;

        [Display(Name = "Hired Date")]
        [DataType(DataType.Date)]
        public DateTime HiredDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Province")]
        public string Province { get; set; } = null!;

        public List<SelectListItem> Provinces { get; set; } = new();

        [Required]
        [Display(Name = "City")]
        public string City { get; set; } = null!;

        public List<SelectListItem> Cities { get; set; } = new();

        [Required]
        [Display(Name = "Warehouse")]
        public int? WarehouseId { get; set; }
        public string? Warehouse { get; set; } // for displaying warehouse name
        public List<SelectListItem> Warehouses { get; set; } = new();

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Application User Id")]
        public string? ApplicationUserId { get; set; }

        [Display(Name = "Role")]
        public string? SelectedRole { get; set; }

        public IEnumerable<SelectListItem>? Roles { get; set; }
        public IEnumerable<string>? RoleNames { get; set; }

        [EmailAddress]
        [Display(Name = "Username (Email)")]
        public string? Username { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        // Search / Filter fields
        public string? SearchText { get; set; }
        public string? StatusFilter { get; set; }
    }
}
