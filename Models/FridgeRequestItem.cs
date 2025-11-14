using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class FridgeRequestItem
    {
        [Key]

        public int FridgeRequestItemId { get; set; }
        public int? CustomerFridgeId { get; set; }
        public CustomerFridge CustomerFridge { get; set; }
        //fk to fridge request
        public int RequestId { get; set; }
        public virtual FridgeRequest FridgeRequest { get; set; }
        //public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public string? SerialNumber { get; set; } // The actual serial number of *this* physical fridge

        //fk to fridges 
        public int FridgeId { get; set; }
        public virtual Fridge Fridge { get; set; }
    // public string? FridgeName { get; set; }

        [Required]
        [Range(1, int.MaxValue)] // Quantity must be at least 1
        public int Quantity { get; set; }
    }
}
