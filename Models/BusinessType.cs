using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class BusinessType
    {
        public int BusinessTypeId { get; set; }

        [Required]
        [Display(Name = "Business Type")]
        public string TypeName { get; set; }

        // Navigation property: one business type can have many customers
        public ICollection<Customer>? Customers { get; set; }
    }
}
