using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models;
[NotMapped] // ✅ Tells EF Core to ignore this model

public class CartItem
{
    public int FridgeId { get; set; }
    public string FridgeName { get; set; }
    public string FridgeImageUrl { get; set; }
    public int Quantity { get; set; } // For fridges, this will likely always be 1
    public int AvailableStock { get; set; } // To display "Only 1 left in stock"
    public FridgeCategory SelectedCategory { get; set; }
    public List<SelectListItem>? CategoryOptions { get; set; }
}
