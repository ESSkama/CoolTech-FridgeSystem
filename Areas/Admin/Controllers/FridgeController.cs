using FridgeSystem.Data;
using FridgeSystem.Migrations;
using FridgeSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FridgeSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")] //allows only the admin to do this
    [Area("Admin")]
    public class FridgeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserController> _logger;

        public FridgeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: Admin/Fridge
        public async Task<IActionResult> Index()
        {
            var fridges = await _context.Fridges
                                        .Include(f => f.Supplier)
                                        .Include(f => f.Warehouse)
                                        .Where(f => f.isDeleted == false || f.isDeleted == null) // Show non-deleted fridges
                                        .OrderBy(f => f.Model)
                                        .ToListAsync();
            return View(fridges);
        }

        // GET: Admin/Fridge/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fridge = await _context.Fridges
                                       .Include(f => f.Supplier)
                                       .Include(f => f.Supplier)
                                       .FirstOrDefaultAsync(m => m.FridgeId == id);
            if (fridge == null)
            {
                return NotFound();
            }

            return View(fridge);
        }

        // GET: Admin/Fridge/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new FridgeCreateEditViewModel
            {
                // Populate Suppliers dropdown
                Suppliers = await _context.Suppliers
                                        .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.SupplierName })
                                        .ToListAsync(),
                Warehouses = await _context.Warehouses
                                        .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                                         .ToListAsync(),
                // Set default values if needed
                IsAvailable = true,
                PurchaseDate = DateTime.Today,
                DeliveryDate = DateTime.Today
            };
            return View(viewModel);
        }

        // POST: Admin/Fridge/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FridgeCreateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var fridge = new Fridge
                {
                    Model = viewModel.Model,
                    SerialNumber = viewModel.SerialNumber,
                    Specification = viewModel.Specification,
                    FridgeColor = viewModel.FridgeColor,
                    FridgeImage = viewModel.FridgeImage,
                    StockQuantity = viewModel.StockQuantity,
                    IsAvailable = viewModel.IsAvailable,
                    PurchaseDate = viewModel.PurchaseDate,
                    WarrantyExpiryDate = viewModel.WarrantyExpiryDate,
                    DeliveryDate = viewModel.DeliveryDate,
                    Category = viewModel.Category,
                    WarehouseId = viewModel.WarehouseId,
                    SupplierId = viewModel.SelectedSupplierId,
                    isDeleted = false
                };

                _context.Add(fridge);
                await _context.SaveChangesAsync();

                // Employee Notification (Inventory Liaisons)
                var allActiveEmployees = await _context.Employees
                    .Where(e => e.IsActive && !string.IsNullOrEmpty(e.ApplicationUserId))
                    .ToListAsync();

                foreach (var employee in allActiveEmployees)
                {
                    var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("Inventory Liaison"))
                        {
                            var notification = new EmployeeNotification
                            {
                                EmployeeId = employee.EmployeeId,
                                Message = $"A new fridge '{fridge.Model}' has been added to the inventory.",
                                SentDate = DateTime.UtcNow
                            };
                            _context.EmployeesNotifications.Add(notification);
                        }
                    }
                }

                await _context.SaveChangesAsync(); // Save all notifications at once
                TempData["SuccessMessage"] = $"Fridge '{fridge.Model}' added successfully!";
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, re-populate suppliers before returning the view
            viewModel.Suppliers = await _context.Suppliers
                                                .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.SupplierName })
                                                .ToListAsync();
            viewModel.Warehouses = await _context.Warehouses
                                      .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                                      .ToListAsync();
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            return View("Index","Fridge");
        }


        // GET: Admin/Fridge/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {



            if (id == null)
            {
                return NotFound();
            }

            var fridge = await _context.Fridges.FindAsync(id);
            if (fridge == null)
            {
                return NotFound();
            }

            var viewModel = new FridgeCreateEditViewModel
            {
                FridgeId = fridge.FridgeId,
                Model = fridge.Model,
                SerialNumber = fridge.SerialNumber,
                Specification = fridge.Specification,
                FridgeColor = fridge.FridgeColor,
                FridgeImage = fridge.FridgeImage,
                StockQuantity = fridge.StockQuantity,
                IsAvailable = fridge.IsAvailable ?? false, // Handle nullable bool
                PurchaseDate = fridge.PurchaseDate,
                WarrantyExpiryDate = fridge.WarrantyExpiryDate,
                DeliveryDate = fridge.DeliveryDate,
                Category = fridge.Category,
                WarehouseId = fridge.WarehouseId,
                SelectedSupplierId = fridge.SupplierId,
                Suppliers = await _context.Suppliers
                                        .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.SupplierName })
                                        .ToListAsync(),
                Warehouses = await _context.Warehouses
                                        .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                                        .ToListAsync()
            };
            return View(viewModel);
        }

        // POST: Admin/Fridge/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FridgeCreateEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values
                                               .SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = "ModelState errors: " + errors;
                return View(viewModel);
            }

            if (id != viewModel.FridgeId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var fridge = await _context.Fridges.FindAsync(id);
                    if (fridge == null)
                        return NotFound();

                    // Update properties from ViewModel
                    fridge.Model = viewModel.Model;
                    fridge.SerialNumber = viewModel.SerialNumber;
                    fridge.Specification = viewModel.Specification;
                    fridge.FridgeColor = viewModel.FridgeColor;
                    fridge.StockQuantity = viewModel.StockQuantity;
                    fridge.IsAvailable = viewModel.IsAvailable;
                    fridge.PurchaseDate = viewModel.PurchaseDate;
                    fridge.WarrantyExpiryDate = viewModel.WarrantyExpiryDate;
                    fridge.DeliveryDate = viewModel.DeliveryDate;
                    fridge.Category = viewModel.Category;
                    fridge.SupplierId = viewModel.SelectedSupplierId;
                    fridge.WarehouseId = viewModel.WarehouseId;

                    // Only update the image if a new file was uploaded
                    if (viewModel.FridgeImageFile != null && viewModel.FridgeImageFile.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}_{viewModel.FridgeImageFile.FileName}";
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.FridgeImageFile.CopyToAsync(stream);
                        }

                        fridge.FridgeImage = "/images/" + fileName;
                    }

                    _context.Update(fridge);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Fridge '{fridge.Model}' updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FridgeExists(viewModel.FridgeId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction("Index", "Fridge");
            }

            // Re-populate suppliers if model state is invalid
            viewModel.Suppliers = await _context.Suppliers
                                                .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.SupplierName })
                                                .ToListAsync();
            viewModel.Warehouses = await _context.Warehouses
                                       .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                                       .ToListAsync();
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            return View(viewModel);
        }


        // GET: Admin/Fridge/Delete/5 (Optional: display a confirmation page)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fridge = await _context.Fridges
                                       .Include(f => f.Supplier)
                                       .Include(f => f.Warehouse)
                                       .FirstOrDefaultAsync(m => m.FridgeId == id);
            if (fridge == null)
            {
                return NotFound();
            }

            return View(fridge);
        }

        // POST: Admin/Fridge/Delete/5 (Actual deletion logic)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fridge = await _context.Fridges.FindAsync(id);
            if (fridge != null)
            {
                // Soft delete: Mark as deleted instead of actual removal
                fridge.isDeleted = true;
                _context.Update(fridge);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Fridge '{fridge.Model}' marked as deleted.";
            }
            else
            {
                TempData["ErrorMessage"] = "Fridge not found.";
            }
            return RedirectToAction(nameof(Index));
        }


        // Hard Delete (use with caution, only if you want to permanently remove)
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HardDelete(int id)
        {
            var fridge = await _context.Fridges.FindAsync(id);
            if (fridge != null)
            {
                _context.Fridges.Remove(fridge);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Fridge '{fridge.Model}' permanently deleted.";
            }
            else
            {
                TempData["ErrorMessage"] = "Fridge not found.";
            }
            return RedirectToAction(nameof(Index));
        }
        */


        private bool FridgeExists(int id)
        {
            return _context.Fridges.Any(e => e.FridgeId == id);
        }
    }
}
