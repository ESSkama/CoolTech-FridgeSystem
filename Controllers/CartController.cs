using FridgeSystem.Data;
using FridgeSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace FridgeSystem.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "CartSession";
        private const string CartRequestTempDataKey = "CartRequestItems"; // <-- NEW CONSTANT for TempData

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get cart from session
        // Get cart from session
        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        // Save cart to session
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }

        // Display cart
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Adds fridges to the cart (example method)
        [HttpPost] // Typically AddToCart would be a POST
        [ValidateAntiForgeryToken] // Recommended for POST actions
        public IActionResult AddToCart(int fridgeId, int quantity = 1) // Added quantity parameter
        {
            if (quantity < 1)
            {
                TempData["ErrorMessage"] = "Quantity must be at least 1.";
                return RedirectToAction("Gallery", "Home"); // Or wherever your fridge display is
            }

            var cart = GetCart();

            var fridge = _context.Fridges.FirstOrDefault(f => f.FridgeId == fridgeId);
            if (fridge == null)
            {
                TempData["ErrorMessage"] = "Fridge not found!";
                return RedirectToAction("Gallery", "Home");
            }

            // Check if item already in cart, update quantity
            var existingItem = cart.FirstOrDefault(c => c.FridgeId == fridgeId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    FridgeId = fridge.FridgeId,
                    FridgeName = fridge.Model,
                    FridgeImageUrl = fridge.FridgeImage,
                    Quantity = quantity,
                    SelectedCategory = fridge.Category
                });
            }

            SaveCart(cart);
            TempData["SuccessMessage"] = $"{fridge.Model} added to cart!";
            return RedirectToAction("Index"); // Redirect to cart index after adding
        }


        // Remove from cart
        [HttpPost] // Remove action should also be a POST for security/idempotency
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int fridgeId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.FridgeId == fridgeId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
                TempData["SuccessMessage"] = $"{item.FridgeName} removed from cart.";
            }
            return RedirectToAction("Index");
        }

        // Update quantity in cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCartQuantity(int fridgeId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.FridgeId == fridgeId);
            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                    SaveCart(cart);
                    TempData["SuccessMessage"] = $"{item.FridgeName} quantity updated.";
                }
                else
                {
                    // If quantity is 0 or less, remove item
                    cart.Remove(item);
                    SaveCart(cart);
                    TempData["SuccessMessage"] = $"{item.FridgeName} removed from cart.";
                }
            }
            return RedirectToAction("Index");
        }


        // Clear cart
        [HttpPost] // Clear cart should also be a POST
        [ValidateAntiForgeryToken]
        public IActionResult ClearCart()
        {
            SaveCart(new List<CartItem>());
            TempData["SuccessMessage"] = "Your cart has been cleared.";
            return RedirectToAction("Index");
        }

        public IActionResult CreateRequest()
        {
            var cart = GetCart();
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty! Please add fridges before creating a request.";
                return RedirectToAction("Index"); // Redirect back to empty cart view
            }

            // --- CRITICAL PART: Setting TempData with the cart items ---
            var cartJson = JsonConvert.SerializeObject(cart);

            // Add Console.WriteLine here for debugging
            Console.WriteLine($"CartController.CreateRequest: Setting TempData['{CartRequestTempDataKey}']. JSON length: {cartJson.Length}");
            Console.WriteLine($"CartController.CreateRequest: JSON content: {cartJson}"); // Be careful with very large outputs

            TempData[CartRequestTempDataKey] = cartJson; // Use the consistent constant key

            return RedirectToAction("Create", "CustomerRequests"); // Redirect to the request form
        }



    }
}
