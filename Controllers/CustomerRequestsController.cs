using Azure.Core;
using FridgeSystem.Data;
using FridgeSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic; 
using System.Threading.Tasks;     
using System;
using System.Security.Claims;

namespace FridgeSystem.Controllers
{
    [Authorize(Roles ="Customer")]
    public class CustomerRequestsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string CartSessionKey = "CartSession"; // Using the same constant as CartController
        private const string CartRequestTempDataKey = "CartRequestItems"; // <-- Define this constant here too

        public CustomerRequestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Helper method to get available fridges for gallery display
        private async Task<List<AvailableFridgeDisplayModel>> GetAvailableFridgesForDisplayAsync()
        {
            // This method would now typically be used to get ALL fridges that have stock,
            // not specifically for a category, as category filtering is done via AJAX.
            // Or you can modify it to accept a category parameter.
            // For now, let's keep it if it's used elsewhere for a general gallery.
            // If it's only for the Create view, the AJAX call will replace its initial use.

            var stockSummaries = await _context.WarehouseStocks
                .Where(ws => ws.Quantity > 0 && ws.FridgeId != null)
                .GroupBy(ws => ws.FridgeId)
                .Select(g => new
                {
                    FridgeId = g.Key,
                    TotalAvailableQuantity = g.Sum(ws => ws.Quantity)
                })
                .ToListAsync();

            var fridgeIds = stockSummaries.Select(s => s.FridgeId).ToList();

            var fridges = await _context.Fridges
                .Include(f => f.Supplier) // Include supplier
                .Where(f => fridgeIds.Contains(f.FridgeId) && f.isDeleted != true)
                .ToListAsync();

            var result = fridges
                .Join(stockSummaries,
                      f => f.FridgeId,
                      ss => ss.FridgeId,
                      (f, ss) => new AvailableFridgeDisplayModel
                      {
                          FridgeId = f.FridgeId,
                          Model = f.Model,
                          Category = f.Category,
                          Specification = f.Specification,
                          ImageUrl = f.FridgeImage,
                          AvailableQuantity = ss.TotalAvailableQuantity,
                          SupplierName = f.Supplier.SupplierName // Add supplier name
                      })
                .OrderBy(f => f.Model)
                .ToList();

            return result;
        }
        // GET: CustomerRequests/Index - View all current/past requests for the logged-in customer
        
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers
                                        .Include(c => c.ApplicationUser)
                                        .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer profile not found. Please contact support.";
                return RedirectToAction("Login", "Account");
            }

            var customerRequests = await _context.FridgeRequests
                                                .Where(fr => fr.BusinessId == customer.BusinessId)
                                                .Include(fr => fr.Warehouse)
                                                .Include(fr => fr.FridgeRequestItems)
                                                    .ThenInclude(fri => fri.Fridge)
                                                .OrderByDescending(fr => fr.RequestDate)
                                                .ToListAsync();

            return View(customerRequests);
        }

        // GET: CustomerRequests/Create - Displays the request form with cart items
        //GET: CustomerRequests/Create - Displays the request form with cart items
        public async Task<IActionResult> Create()
        {
            // TempData.Keep() ensures the TempData item persists for another request,
            // which is useful if the form is re-rendered due to a validation error on POST.
            // It does no harm to keep it here for the initial GET as well.
            TempData.Keep(CartRequestTempDataKey);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer profile not found. Please contact support.";
                return RedirectToAction("Login", "Account");
            }

            // Initialize ViewModel
            var model = new FridgeRequestViewModel
            {
                ContactNumber = customer.Telephone,
                ContactPerson = customer.ContactPersonName,
                PreferredPickup = DateTime.Today.AddDays(1), // Default to tomorrow
                RequestedFridges = new List<FridgeRequestItemViewModel>() // Always initialize the list
            };

            // Populate all dropdown options using the helper method
            await PopulateDropdownsForViewModel(model);

            // Set default selected values for dropdowns after they have been populated
            model.SelectedWarehouseId = model.WarehouseOptions.FirstOrDefault()?.Value != null
                                        ? int.Parse(model.WarehouseOptions.First().Value) : 0;
            model.SelectedBranchId = model.BranchOptions.FirstOrDefault()?.Value != null
                                     ? int.Parse(model.BranchOptions.First().Value) : 0;


            // --- Load cart items from TempData ---
            // This section attempts to retrieve and deserialize the cart items passed via TempData.
            if (TempData[CartRequestTempDataKey] is string cartJson && !string.IsNullOrEmpty(cartJson))
            {
                Console.WriteLine($"CustomerRequestsController (GET - Create): TempData['{CartRequestTempDataKey}'] received. JSON length: {cartJson.Length}");

                try
                {
                    var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

                    if (cartItems != null && cartItems.Any())
                    {
                        model.RequestedFridges = cartItems.Select(c => new FridgeRequestItemViewModel
                        {
                            SelectedFridgeId = c.FridgeId,
                            Quantity = c.Quantity,
                            Model = c.FridgeName,
                            ImageUrl = c.FridgeImageUrl,
                            SelectedCategory = c.SelectedCategory
                        }).ToList();
                        Console.WriteLine($"CustomerRequestsController (GET - Create): Deserialized {model.RequestedFridges.Count} items into model.RequestedFridges.");
                    }
                    else
                    {
                        // Deserialization successful, but the list was empty or null
                        Console.WriteLine("CustomerRequestsController (GET - Create): TempData['CartRequestItems'] deserialized to an empty or null list of cart items.");
                        TempData["ErrorMessage"] = "Your cart appears to be empty or corrupted. Please add fridges again.";
                    }
                }
                catch (JsonException ex)
                {
                    // Error during JSON deserialization
                    Console.Error.WriteLine($"CustomerRequestsController (GET - Create): ERROR deserializing TempData['{CartRequestTempDataKey}']: {ex.Message}");
                    TempData["ErrorMessage"] = "There was an error loading your cart items. Please try again.";
                    // Clear the corrupted TempData item to prevent future errors
                    TempData[CartRequestTempDataKey] = null;
                }
            }
            else
            {
                // TempData item was null or not a string (e.g., first visit without coming from cart)
                Console.WriteLine($"CustomerRequestsController (GET - Create): TempData['{CartRequestTempDataKey}'] is null or empty. Cart items will not be displayed.");
                TempData["InfoMessage"] = "Your cart is empty. Please add fridges to create a request."; // Informative message for the user
            }

            return View(model);
        }




        [HttpGet]
        public async Task<IActionResult> GetFridgesByCategory(FridgeCategory category)
        {
            // Fetch fridges that are not deleted and are available in stock
            var fridges = await _context.Fridges
                .Include(f => f.Supplier) // Include supplier to get supplier name
                .Where(f => f.Category == category && (f.isDeleted == null || f.isDeleted == false))
                .ToListAsync(); // Get all fridges first

            // Now, join with WarehouseStocks to get actual available quantity
            var fridgeIdsInStock = await _context.WarehouseStocks
                .Where(ws => ws.Quantity > 0 && ws.FridgeId != null)
                .GroupBy(ws => ws.FridgeId)
                .Select(g => new { FridgeId = g.Key, TotalQuantity = g.Sum(ws => ws.Quantity) })
                .ToDictionaryAsync(x => x.FridgeId, x => x.TotalQuantity);

            var result = fridges
                .Where(f => fridgeIdsInStock.ContainsKey(f.FridgeId)) // Only include fridges that have stock
                .Select(f => new AvailableFridgeDisplayModel // Map to your display model
                {
                    FridgeId = f.FridgeId,
                    Model = f.Model,
                    Category = f.Category,
                    Specification = f.Specification,
                    ImageUrl = f.FridgeImage,
                    AvailableQuantity = fridgeIdsInStock[f.FridgeId], // Get quantity from dictionary
                    SupplierName = f.Supplier.SupplierName // Include supplier name
                })
                .OrderBy(f => f.Model)
                .ToList();

            return Json(result);
        }

        // POST: CustomerRequests/Create - Submits the new fridge request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FridgeRequestViewModel model)
        {
            // Re-populate dropdowns if ModelState is invalid to ensure they are not empty on re-render
            await PopulateDropdownsForViewModel(model);

            /* if (!ModelState.IsValid || model.RequestedFridges == null || !model.RequestedFridges.Any())
             {
                 TempData["ErrorMessage"] = "Please add fridges to your cart before submitting, and ensure all required fields are filled.";
                 // TempData["CartRequestItems"] might be needed if the user navigates away and comes back due to validation errors
                 // or if you want to explicitly keep it for re-rendering (though model binding should handle RequestedFridges for POST).
                 return View(model);
             }*/
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is NOT valid. Errors:");
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Any())
                    {
                        Console.WriteLine($"  Field: {state.Key}, Errors: {string.Join("; ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Customer profile not found. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }

                var fridgeRequest = new FridgeRequest
                {
                    BusinessId = customer.BusinessId,
                    CustomerId = userId, // Assign the logged-in user's ID
                    WarehouseId = model.SelectedWarehouseId,
                    CustomerLocationId = model.SelectedBranchId, // Assign the selected branch ID
                    PreferredPickup = model.PreferredPickup,
                    SpecialInstructions = model.SpecialInstructions,
                    ContactNumber = model.ContactNumber,
                    ContactPerson = model.ContactPerson,
                    RequestDate = DateTime.Now,
                    Status = RequestStatus.Pending // Set initial status
                };

                _context.FridgeRequests.Add(fridgeRequest);
                await _context.SaveChangesAsync(); // Save to get the RequestId

                foreach (var item in model.RequestedFridges)
                {
                    // Make sure item.SelectedFridgeId exists in the Fridges table
                    var fridgeExists = await _context.Fridges.AnyAsync(f => f.FridgeId == item.SelectedFridgeId);
                    if (!fridgeExists)
                    {
                        TempData["ErrorMessage"] = $"Fridge with ID {item.SelectedFridgeId} does not exist.";
                        return View(model);
                    }

                    _context.FridgeRequestItems.Add(new FridgeRequestItem
                    {
                        RequestId = fridgeRequest.RequestId,
                        FridgeId = item.SelectedFridgeId,
                        Quantity = item.Quantity
                    });
                }
                await _context.SaveChangesAsync();
                
                HttpContext.Session.Remove(CartSessionKey); // Clear cart session after successful request

                TempData["SuccessMessage"] = "Fridge request submitted successfully!";
                return RedirectToAction("Index"); // Redirect to customer requests index
            }
            return View(model);

        }

        // Helper to populate dropdowns, useful for both GET and POST (on ModelState.IsValid = false)
        // Example of the PopulateDropdownsForViewModel helper (should already exist)
        private async Task PopulateDropdownsForViewModel(FridgeRequestViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            model.WarehouseOptions = await _context.Warehouses
                .OrderBy(w => w.Name)
                .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = $"{w.Name} ({w.City})" })
                .ToListAsync();

            if (customer != null)
            {
                model.BranchOptions = await _context.CustomerLocations
                    .Where(b => b.BusinessId == customer.BusinessId)
                    .OrderBy(b => b.BranchName)
                    .Select(b => new SelectListItem { Value = b.CustomerLocationId.ToString(), Text = $"{b.BranchName} ({b.City})" })
                    .ToListAsync();
            }
            else
            {
                model.BranchOptions = new List<SelectListItem>(); // Empty if no customer found
            }

            model.CategoryOptions = Enum.GetValues(typeof(FridgeCategory))
                .Cast<FridgeCategory>()
                .Select(c => new SelectListItem { Value = ((int)c).ToString(), Text = c.ToString() })
                .ToList();
        }





        // GET: CustomerRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers
                                        .Include(c => c.ApplicationUser)
                                        .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer profile not found. Please contact support.";
                return RedirectToAction("Login", "Account");
            }

            var fridgeRequest = await _context.FridgeRequests
                                            .Include(fr => fr.Warehouse)
                                             .Include(fr => fr.CustomerLocation) // Include CustomerLocation

                                            .Include(fr => fr.FridgeRequestItems)
                                                .ThenInclude(fri => fri.Fridge)
                                            .FirstOrDefaultAsync(m => m.RequestId == id && m.BusinessId == customer.BusinessId);

            if (fridgeRequest == null) return NotFound();

            return View(fridgeRequest);
        }

        [HttpPost]

        public async Task<IActionResult> SubmitRequest(FridgeRequest model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.CustomerId = userId;
                model.RequestDate = DateTime.UtcNow;

                _context.FridgeRequests.Add(model);
                await _context.SaveChangesAsync();

                // Optional: send email to admin

                TempData["SuccessMessage"] = "Your fridge request has been submitted.";
                return RedirectToAction("Index", "CustomerRequests");
            }
            return View(model);
        }



        //CUSTOMER LOCATIONS

        // GET: CustomerLocations
        public async Task<IActionResult> CustomerLocations(string? searchText)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers
                .Include(c => c.CustomerLocations)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null)
                return NotFound("Customer profile not found.");

            var locations = customer.CustomerLocations.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim().ToLower();
                locations = locations.Where(l =>
                    l.BranchName.ToLower().Contains(searchText) ||
                    l.AddressLine1.ToLower().Contains(searchText) ||
                    (l.AddressLine2 != null && l.AddressLine2.ToLower().Contains(searchText)) ||
                    l.City.ToLower().Contains(searchText) ||
                    l.Province.ToLower().Contains(searchText) ||
                    l.Postcode.ToLower().Contains(searchText) ||
                    (l.Telephone != null && l.Telephone.ToLower().Contains(searchText)) ||
                    (l.ContactPerson != null && l.ContactPerson.ToLower().Contains(searchText))
                );
            }

            var model = locations.Select(l => new CustomerLocationViewModel
            {
                LocationId = l.CustomerLocationId,
                BusinessId = customer.BusinessId,
                BranchName = l.BranchName,
                AddressLine1 = l.AddressLine1,
                AddressLine2 = l.AddressLine2,
                City = l.City,
                Province = l.Province,
                Postcode = l.Postcode,
                Telephone = l.Telephone,
                ContactPerson = l.ContactPerson,
                IsActive = l.IsActive,
                BusinessName = customer.BusinessName
            }).ToList();

            ViewBag.CustomerName = customer.BusinessName;
            ViewBag.BusinessId = customer.BusinessId;
            ViewBag.SearchText = searchText;

            return View("CustomerLocations", model);
        }

        // GET: AddLocation
        [HttpGet]
        public async Task<IActionResult> AddLocation()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null)
                return NotFound();

            ViewBag.BusinessId = customer.BusinessId;
            ViewBag.CustomerName = customer.BusinessName;

            var model = new CustomerLocationViewModel
            {
                BusinessId = customer.BusinessId,
                IsActive = true
            };

            return View("AddCustomerLocation", model);
        }

        // POST: AddLocation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLocation(CustomerLocationViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.BusinessId = customer.BusinessId;
                ViewBag.CustomerName = customer.BusinessName;
                return View("AddCustomerLocation", model);
            }

            var location = new CustomerLocation
            {
                BusinessId = customer.BusinessId,
                BranchName = model.BranchName,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                City = model.City,
                Province = model.Province,
                Postcode = model.Postcode,
                Telephone = model.Telephone,
                ContactPerson = model.ContactPerson,
                IsActive = model.IsActive
            };

            _context.CustomerLocations.Add(location);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Branch added successfully!";
            return RedirectToAction(nameof(CustomerLocations));
        }

        // GET: EditLocation
        [HttpGet]
        public async Task<IActionResult> EditLocation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null)
                return NotFound();

            var location = await _context.CustomerLocations
                .FirstOrDefaultAsync(l => l.CustomerLocationId == id && l.BusinessId == customer.BusinessId);

            if (location == null)
                return NotFound();

            var model = new CustomerLocationViewModel
            {
                LocationId = location.CustomerLocationId,
                BusinessId = location.BusinessId,
                BranchName = location.BranchName,
                AddressLine1 = location.AddressLine1,
                AddressLine2 = location.AddressLine2,
                City = location.City,
                Province = location.Province,
                Postcode = location.Postcode,
                Telephone = location.Telephone,
                ContactPerson = location.ContactPerson,
                IsActive = location.IsActive
            };

            ViewBag.BusinessId = customer.BusinessId;
            ViewBag.CustomerName = customer.BusinessName;

            return View("EditCustomerLocation", model);
        }

        // POST: EditLocation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLocation(int id, CustomerLocationViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null)
                return NotFound();

            if (id != model.LocationId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.BusinessId = customer.BusinessId;
                ViewBag.CustomerName = customer.BusinessName;
                return View("EditCustomerLocation", model);
            }

            var location = await _context.CustomerLocations
                .FirstOrDefaultAsync(l => l.CustomerLocationId == id && l.BusinessId == customer.BusinessId);

            if (location == null)
                return NotFound();

            location.BranchName = model.BranchName;
            location.AddressLine1 = model.AddressLine1;
            location.AddressLine2 = model.AddressLine2;
            location.City = model.City;
            location.Province = model.Province;
            location.Postcode = model.Postcode;
            location.Telephone = model.Telephone;
            location.ContactPerson = model.ContactPerson;
            location.IsActive = model.IsActive;

            _context.Update(location);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Branch updated successfully!";
            return RedirectToAction(nameof(CustomerLocations));
        }

        public async Task<IActionResult> ApprovedFridges()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Get the current logged-in customer's record
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null)
                return NotFound("Customer not found.");

            // Get all fridges assigned to this customer (approved requests)
            var approvedFridges = await _context.CustomerFridges
                .Include(cf => cf.Fridge)               // Load fridge model info
                .Include(cf => cf.CustomerLocation)     // Load branch/location info
                .Where(cf => cf.CustomerLocation.BusinessId == customer.BusinessId)
                .ToListAsync();

            return View(approvedFridges); // Send CustomerFridge list to the view
        }

        public async Task<IActionResult> ApprovedFridgesDetails(int id)
        {
            var customerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var request = await _context.FridgeRequests
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.FridgeRequestItems)
                    .ThenInclude(i => i.Fridge)
                .Include(r => r.Customer)
                .Include(r => r.Warehouse)
                .Include(r => r.CustomerLocation)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null)
                return NotFound();

            return View(request);
        }


        // GET: CustomerRequests/Details/5
      

    }


















}

