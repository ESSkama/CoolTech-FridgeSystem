using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }
        [Required]
        [StringLength(100)]
        public string SupplierName { get; set; }
        [StringLength(200)]
        public string? ContactPerson { get; set; }
        [Phone]
        [StringLength(20)]
        public string? Telephone { get; set; }
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        [StringLength(250)]
        public string? Address { get; set; }
        public bool IsActive { get; set; } = true; // For disabling suppliers without deleting them

        public virtual ICollection<Fridge> SuppliedFridges { get; set; } = new List<Fridge>(); // Fridges supplied by this supplier
        public ICollection<FridgeInventory>? FridgeModels { get; set; }

    }
}
