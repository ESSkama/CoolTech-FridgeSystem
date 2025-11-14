using FridgeSystem.Areas.Admin.Controllers;
using FridgeSystem.Data;
using FridgeSystem.Models;
using FridgeSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FridgeSystem.Controllers
{
    [Authorize(Roles = "Inventory Liaison")]
    public class InventoryLiaisonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserController> _logger;

        public InventoryLiaisonController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<UserController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Helper: get current Employee
        private async Task<Employee> GetCurrentEmployeeAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
        }

        // DASHBOARD
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            // Get logged-in employee
            var liaison = await _context.Employees
                .Include(e => e.Notifications)
                .FirstOrDefaultAsync(e => e.ApplicationUserId == userId);

            if (liaison == null) return Unauthorized();

            // Recent unread notifications only (latest 5)
            var recentNotifications = liaison.Notifications
                .Where(n => !n.IsRead) 
                .OrderByDescending(n => n.SentDate)
                .Take(5)
                .Select(n => new EmployeeNotificationViewModel
                {
                    NotificationId = n.NotificationId,
                    Message = n.Message,
                    SentDate = n.SentDate,
                    IsRead = n.IsRead
                })
                .ToList();

            // Total fridges managed by this liaison
            var totalFridges = await _context.FridgesInventory
                .CountAsync(f => f.InventoryLiasonEmployeeId == liaison.EmployeeId);

            // Low stock alerts (<= 5 units)
            var lowStockList = await _context.FridgesInventory
                .Where(f => f.InventoryLiasonEmployeeId == liaison.EmployeeId && f.QuantityInStock <= 5)
                .Select(f => new StockAlertViewModel
                {
                    InventoryId = f.InventoryId,
                    Model = f.Model,
                    Category = f.Category.ToString(),
                    QuantityInStock = f.QuantityInStock,
                    SupplierName = f.Supplier != null ? f.Supplier.SupplierName : "Unknown"
                })
                .ToListAsync();

            var lowStockCount = lowStockList.Count;

            // Pending purchase requests for this liaison's inventory
            var pendingRequests = await _context.PurchaseRequests
                .Where(r => r.Inventory.InventoryLiasonEmployeeId == liaison.EmployeeId &&
                            r.RequestStatus == RequestStatus.Pending)
                .CountAsync();

            // Unread notifications count
            var unreadNotifications = liaison.Notifications.Count(n => !n.IsRead);

            // Recent fridge additions (latest 5)
            var recentFridgeAdditions = await _context.FridgesInventory
                .Where(f => f.InventoryLiasonEmployeeId == liaison.EmployeeId)
                .OrderByDescending(f => f.LastUpdate)
                .Take(5)
                .Select(f => f.Model)
                .ToListAsync();

            // Build dashboard view model
            var dashboardModel = new InvLiaisonDashboardViewModel
            {
                EmployeeName = $"{liaison.FirstName} {liaison.LastName}",
                TotalFridgesInInventory = totalFridges,
                LowStockAlerts = lowStockCount,
                PendingPurchaseRequests = pendingRequests,
                UnreadNotifications = unreadNotifications,
                RecentFridgeAdditions = recentFridgeAdditions,
                RecentNotifications = recentNotifications, 
                LowStockAlertsList = lowStockList
            };

            return View(dashboardModel);
        }



        // GET: Employee Notifications
        public async Task<IActionResult> EmployeeNotifications()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var employee = await _context.Employees
                .Include(e => e.Notifications)
                .FirstOrDefaultAsync(e => e.ApplicationUserId == userId);

            if (employee == null) return Unauthorized();

            var model = employee.Notifications
                .OrderByDescending(n => n.SentDate)
                .Select(n => new EmployeeNotificationViewModel
                {
                    NotificationId = n.NotificationId,
                    Message = n.Message,
                    SentDate = n.SentDate,
                    IsRead = n.IsRead
                })
                .ToList();

            return View("EmployeeNotification", model);
        }

        // POST: Mark single notification as read
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.EmployeesNotifications.FindAsync(id);
            if (notification == null) return NotFound();

            notification.IsRead = true;
            _context.Update(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EmployeeNotifications));
        }

        // POST: Mark all notifications as read
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var employee = await _context.Employees
                .Include(e => e.Notifications)
                .FirstOrDefaultAsync(e => e.ApplicationUserId == userId);

            if (employee == null) return Unauthorized();

            foreach (var n in employee.Notifications.Where(n => !n.IsRead))
                n.IsRead = true;

            _context.UpdateRange(employee.Notifications.Where(n => !n.IsRead));
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EmployeeNotifications));
        }

        // INVENTORY LIST WITH SEARCH/FILTER AND WAREHOUSE
        public async Task<IActionResult> Inventory(string searchModel = null, string searchCategory = null,
                                                   string searchColor = null, string searchType = null,
                                                   string searchWarehouse = null,
                                                   DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            // Store search terms in ViewBag to keep form state
            ViewBag.SearchModel = searchModel ?? "";
            ViewBag.SearchCategory = searchCategory ?? "";
            ViewBag.SearchColor = searchColor ?? "";
            ViewBag.SearchType = searchType ?? "";
            ViewBag.SearchWarehouse = searchWarehouse ?? "";
            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");

            var query = _context.FridgesInventory
                                .Include(i => i.Supplier)
                                .Include(i => i.Fridges)
                                    .ThenInclude(f => f.WarehouseStocks)
                                        .ThenInclude(ws => ws.Warehouse)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel))
                query = query.Where(i => i.Model.Contains(searchModel));

            if (!string.IsNullOrEmpty(searchCategory))
                query = query.Where(i => i.Category.Contains(searchCategory));

            if (!string.IsNullOrEmpty(searchColor))
                query = query.Where(i => i.Fridges.Any(f => f.FridgeColor.Contains(searchColor)));

            if (!string.IsNullOrEmpty(searchType))
                query = query.Where(i => i.Fridges.Any(f => f.Specification.Contains(searchType))); // Use Specification as type

            if (!string.IsNullOrEmpty(searchWarehouse))
                query = query.Where(i => i.Fridges
                                           .Any(f => f.WarehouseStocks
                                                      .Any(ws => ws.Warehouse.Name.Contains(searchWarehouse))));

            if (dateFrom.HasValue)
                query = query.Where(i => i.LastUpdate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(i => i.LastUpdate <= dateTo.Value);

            // Fetch the fridge inventory list using the FILTERED 'query' variable.
            // This ensures all includes (like Warehouse) and all search criteria are applied.
            var model = await query
                .Select(i => new FridgeInventoryViewModel
                {
                    InventoryId = i.InventoryId,
                    FridgeModel = i.Model,
                    FridgeCategory = i.Category,
                    QuantityInStock = i.QuantityInStock,
                    SupplierId = i.SupplierId,

                    // Supplier name (works)
                    SupplierName = i.Supplier != null ? i.Supplier.SupplierName : "N/A",

                    // Warehouse names (fixed)
                    WarehousesNames = string.Join(", ",
                        i.Fridges
                        .SelectMany(f => f.WarehouseStocks)
                        .Where(ws => ws.Warehouse != null)
                        .Select(ws => ws.Warehouse.Name)
                        .Distinct()
                    ),

                    SelectedWarehouseId = i.Fridges
                        .SelectMany(f => f.WarehouseStocks)
                        .Select(ws => ws.WarehouseId)
                        .FirstOrDefault(),

                    // Note: Since Suppliers and WarehousesDropdown are static lists, 
                    // fetching them this way will work, though it's often more efficient 
                    // to fetch them once outside the main query loop.
                    Suppliers = _context.Suppliers
                       .Select(s => new SelectListItem
                       {
                           Value = s.SupplierId.ToString(),
                           Text = s.SupplierName
                       })
                       .ToList(),

                    WarehousesDropdown = _context.Warehouses
                    .Select(w => new SelectListItem
                    {
                        Value = w.WarehouseId.ToString(),
                        Text = w.Name
                    })
                    .ToList()
                })
                .ToListAsync();


            return View("Inventory", model);
        }

            // ADD FRIDGE (Model)
            [HttpGet]
        public IActionResult AddFridge()
        {
            var model = new FridgeInventoryViewModel
            {
                Suppliers = _context.Suppliers
                    .Select(s => new SelectListItem
                    {
                        Value = s.SupplierId.ToString(),
                        Text = s.SupplierName
                    })
                    .ToList(),

                WarehousesDropdown = _context.Warehouses
                    .Select(w => new SelectListItem
                    {
                        Value = w.WarehouseId.ToString(),
                        Text = w.Name
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFridge(FridgeInventoryViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                // Get the logged-in employee
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
                if (employee == null) return Unauthorized();

                // Create the fridge inventory
                var fridgeInventory = new FridgeInventory
                {
                    Model = model.FridgeModel,
                    Category = model.FridgeCategory,
                    QuantityInStock = model.QuantityInStock,
                    SupplierId = model.SupplierId.Value,
                    LastUpdate = DateTime.UtcNow,
                    InventoryLiasonEmployeeId = employee.EmployeeId  // <--- match property name
                };

                _context.FridgesInventory.Add(fridgeInventory);
                await _context.SaveChangesAsync();

                // Link to warehouse if selected
                if (model.SelectedWarehouseId.HasValue)
                {
                    var warehouseStock = new WarehouseStock
                    {
                        WarehouseId = model.SelectedWarehouseId.Value,
                        FridgeId = fridgeInventory.InventoryId,
                        Quantity = model.QuantityInStock
                    };
                    _context.WarehouseStocks.Add(warehouseStock);
                    await _context.SaveChangesAsync();
                }

                // Notification for logged-in employee
                var notification = new EmployeeNotification
                {
                    EmployeeId = employee.EmployeeId,
                    Message = $"New fridge model '{fridgeInventory.Model}' added to inventory.",
                    SentDate = DateTime.UtcNow
                };
                _context.EmployeesNotifications.Add(notification);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Fridge model added successfully!";
                return RedirectToAction(nameof(Inventory));
            }

            // Reload dropdowns if validation fails
            model.Suppliers = _context.Suppliers
                .Select(s => new SelectListItem
                {
                    Value = s.SupplierId.ToString(),
                    Text = s.SupplierName
                })
                .ToList();

            model.WarehousesDropdown = _context.Warehouses
                .Select(w => new SelectListItem
                {
                    Value = w.WarehouseId.ToString(),
                    Text = w.Name
                })
                .ToList();

            return View(model);
        }



        // EDIT FRIDGE (Model)
        [HttpGet]
        public async Task<IActionResult> EditFridge(int inventoryId)
        {
            var fridgeInventory = await _context.FridgesInventory
                .Include(f => f.Supplier)
                .FirstOrDefaultAsync(f => f.InventoryId == inventoryId);

            if (fridgeInventory == null) return NotFound();

            var warehouseStock = await _context.WarehouseStocks
                .FirstOrDefaultAsync(w => w.FridgeId == inventoryId);

            var model = new FridgeInventoryViewModel
            {
                InventoryId = fridgeInventory.InventoryId,
                FridgeModel = fridgeInventory.Model,
                FridgeCategory = fridgeInventory.Category,
                QuantityInStock = fridgeInventory.QuantityInStock,
                SupplierId = fridgeInventory.SupplierId,
                SelectedWarehouseId = warehouseStock?.WarehouseId,

                Suppliers = _context.Suppliers
                    .Select(s => new SelectListItem
                    {
                        Value = s.SupplierId.ToString(),
                        Text = s.SupplierName
                    })
                    .ToList(),

                WarehousesDropdown = _context.Warehouses
                    .Select(w => new SelectListItem
                    {
                        Value = w.WarehouseId.ToString(),
                        Text = w.Name
                    })
                    .ToList()
            };

            return View("EditFridge", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFridge(FridgeInventoryViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                var fridgeInventory = await _context.FridgesInventory.FindAsync(model.InventoryId);
                if (fridgeInventory == null) return NotFound();

                fridgeInventory.Model = model.FridgeModel;
                fridgeInventory.Category = model.FridgeCategory;
                fridgeInventory.QuantityInStock = model.QuantityInStock;
                fridgeInventory.SupplierId = model.SupplierId.Value;
                fridgeInventory.LastUpdate = DateTime.UtcNow;

                _context.Update(fridgeInventory);
                await _context.SaveChangesAsync();

                // Update or create WarehouseStock
                var warehouseStock = await _context.WarehouseStocks
                    .FirstOrDefaultAsync(w => w.FridgeId == model.InventoryId);

                if (warehouseStock != null)
                {
                    warehouseStock.WarehouseId = model.SelectedWarehouseId ?? warehouseStock.WarehouseId;
                    warehouseStock.Quantity = model.QuantityInStock;
                    _context.WarehouseStocks.Update(warehouseStock);
                }
                else if (model.SelectedWarehouseId.HasValue)
                {
                    _context.WarehouseStocks.Add(new WarehouseStock
                    {
                        WarehouseId = model.SelectedWarehouseId.Value,
                        FridgeId = model.InventoryId,
                        Quantity = model.QuantityInStock
                    });
                }

                await _context.SaveChangesAsync();

                // Notification
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
                if (employee != null)
                {
                    var notification = new EmployeeNotification
                    {
                        EmployeeId = employee.EmployeeId,
                        Message = $"Fridge model '{fridgeInventory.Model}' was updated in inventory.",
                        SentDate = DateTime.UtcNow
                    };
                    _context.EmployeesNotifications.Add(notification);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Fridge model updated successfully!";
                return RedirectToAction(nameof(Inventory));
            }

            // Reload dropdowns if validation fails
            model.Suppliers = _context.Suppliers
                .Select(s => new SelectListItem
                {
                    Value = s.SupplierId.ToString(),
                    Text = s.SupplierName
                })
                .ToList();

            model.WarehousesDropdown = _context.Warehouses
                .Select(w => new SelectListItem
                {
                    Value = w.WarehouseId.ToString(),
                    Text = w.Name
                })
                .ToList();

            return View("EditFridge", model);
        }


        // FRIDGE UNIT LIST WITH SEARCH FILTER
        public async Task<IActionResult> FridgeUnit(string? searchTerm, int? warehouseId = null)
        {
            var query = _context.Fridges
                .Include(f => f.Inventory)
                .Include(f => f.WarehouseStocks)
                    .ThenInclude(ws => ws.Warehouse)
                .Where(f => f.isDeleted == false || f.isDeleted == null)
                .AsQueryable();

            // Search by model/type/color/name
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(f =>
                    f.Model.Contains(searchTerm) ||
                    f.FridgeColor.Contains(searchTerm) ||
                    f.Specification.Contains(searchTerm)
                );
            }

            // Filter by warehouse
            if (warehouseId.HasValue)
            {
                query = query.Where(f => f.WarehouseStocks.Any(ws => ws.WarehouseId == warehouseId.Value));
            }

            var model = await query
                .Select(f => new FridgeUnitViewModel
                {
                    FridgeId = f.FridgeId,
                    Model = f.Model,
                    SerialNumber = f.SerialNumber,
                    FridgeDescription = f.Specification,
                    FridgeColor = f.FridgeColor,
                    StockQuantity = f.StockQuantity,
                    IsAvailable = f.IsAvailable ?? false,
                    SupplierName = f.Supplier != null ? f.Supplier.SupplierName : "N/A",
                    WarehouseNames = f.WarehouseStocks.Select(ws => ws.Warehouse.Name).ToList(),

                    // Action buttons
                    EditFridgeUnitAction = Url.Action("EditFridgeUnit", new { id = f.FridgeId }),
                    ScrapFridgeAction = Url.Action("ScrapFridge", new { id = f.FridgeId }),
                    FridgeHistoryAction = Url.Action("FridgeHistory", new { fridgeId = f.FridgeId })
                })
                .ToListAsync();

            ViewBag.Warehouses = await _context.Warehouses
                .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                .ToListAsync();

            return View("FridgeUnit", model);
        }


        // GET: InventoryLiaison/Fridge/Add
        [HttpGet]
        public async Task<IActionResult> AddFridgeUnit()
        {
            var viewModel = new FridgeCreateEditViewModel
            {
                Suppliers = await _context.Suppliers
                    .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.SupplierName })
                    .ToListAsync(),
                IsAvailable = true,
                PurchaseDate = DateTime.Today,
                DeliveryDate = DateTime.Today
            };

            ViewBag.Warehouses = await _context.Warehouses
                .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                .ToListAsync();

            return View(viewModel);
        }

        // POST: InventoryLiaison/Fridge/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFridgeUnit(FridgeCreateEditViewModel viewModel, int? WarehouseId)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload if included
                if (viewModel.FridgeImageFile != null && viewModel.FridgeImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/fridges");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(viewModel.FridgeImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await viewModel.FridgeImageFile.CopyToAsync(stream);

                    viewModel.FridgeImage = "/images/fridges/" + uniqueFileName;
                }

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
                    SupplierId = viewModel.SelectedSupplierId ?? 0,
                    isDeleted = false
                };

                _context.Fridges.Add(fridge);
                await _context.SaveChangesAsync();

                // Optional: add warehouse stock if WarehouseId is provided
                if (WarehouseId.HasValue)
                {
                    _context.WarehouseStocks.Add(new WarehouseStock
                    {
                        WarehouseId = WarehouseId.Value,
                        FridgeId = fridge.FridgeId,
                        Quantity = fridge.StockQuantity
                    });
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"Fridge '{fridge.Model}' added successfully!";
                return RedirectToAction(nameof(FridgeUnit));
            }

            // Re-populate dropdowns if ModelState fails
            viewModel.Suppliers = await _context.Suppliers
                .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.SupplierName })
                .ToListAsync();

            ViewBag.Warehouses = await _context.Warehouses
                .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                .ToListAsync();

            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            return View(viewModel);
        }





        // GET: InventoryLiaison/Fridge/Edit/5
        [HttpGet]
        public async Task<IActionResult> EditFridgeUnit(int id)
        {
            var fridge = await _context.Fridges
                .Include(f => f.WarehouseStocks)
                .FirstOrDefaultAsync(f => f.FridgeId == id && (f.isDeleted == false || f.isDeleted == null));

            if (fridge == null) return NotFound();

            var viewModel = new FridgeCreateEditViewModel
            {
                FridgeId = fridge.FridgeId,
                Model = fridge.Model,
                SerialNumber = fridge.SerialNumber,
                Specification = fridge.Specification,
                FridgeColor = fridge.FridgeColor,
                FridgeImage = fridge.FridgeImage,
                StockQuantity = fridge.StockQuantity,
                IsAvailable = fridge.IsAvailable ?? true,
                PurchaseDate = fridge.PurchaseDate,
                WarrantyExpiryDate = fridge.WarrantyExpiryDate,
                DeliveryDate = fridge.DeliveryDate,
                Category = fridge.Category,
                SelectedSupplierId = fridge.SupplierId ?? 0,
                WarehouseId = fridge.WarehouseStocks.FirstOrDefault()?.WarehouseId,

                Suppliers = await _context.Suppliers
                    .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.SupplierName })
                    .ToListAsync()
            };

            ViewBag.Warehouses = await _context.Warehouses
                .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                .ToListAsync();

            return View("EditFridgeUnit", viewModel);
        }


        // POST: InventoryLiaison/Fridge/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFridgeUnit(FridgeCreateEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns if validation fails
                viewModel.Suppliers = await _context.Suppliers
                    .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.SupplierName })
                    .ToListAsync();

                ViewBag.Warehouses = await _context.Warehouses
                    .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                    .ToListAsync();

                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return View(viewModel);
            }

            var fridge = await _context.Fridges
                .Include(f => f.WarehouseStocks)
                .FirstOrDefaultAsync(f => f.FridgeId == viewModel.FridgeId);

            if (fridge == null) return NotFound();

            // Handle image upload
            if (viewModel.FridgeImageFile != null && viewModel.FridgeImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/fridges");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(viewModel.FridgeImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await viewModel.FridgeImageFile.CopyToAsync(stream);

                fridge.FridgeImage = "/images/fridges/" + uniqueFileName;
            }

            int oldQuantity = fridge.StockQuantity;
            int newQuantity = viewModel.StockQuantity > 0 ? viewModel.StockQuantity : 1;

            // Update fridge properties
            fridge.Model = viewModel.Model;
            fridge.SerialNumber = viewModel.SerialNumber;
            fridge.Specification = viewModel.Specification;
            fridge.FridgeColor = viewModel.FridgeColor;
            fridge.StockQuantity = newQuantity;
            fridge.IsAvailable = viewModel.IsAvailable;
            fridge.PurchaseDate = viewModel.PurchaseDate;
            fridge.WarrantyExpiryDate = viewModel.WarrantyExpiryDate;
            fridge.DeliveryDate = viewModel.DeliveryDate;
            fridge.Category = viewModel.Category;
            fridge.SupplierId = viewModel.SelectedSupplierId ?? 0;
            fridge.isDeleted = false;

            // Update warehouse stock
            var existingStock = fridge.WarehouseStocks.FirstOrDefault();
            if (existingStock != null)
            {
                existingStock.WarehouseId = viewModel.WarehouseId ?? existingStock.WarehouseId;
                existingStock.Quantity = newQuantity;
            }
            else if (viewModel.WarehouseId.HasValue)
            {
                _context.WarehouseStocks.Add(new WarehouseStock
                {
                    FridgeId = fridge.FridgeId,
                    WarehouseId = viewModel.WarehouseId.Value,
                    Quantity = newQuantity
                });
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Fridge '{fridge.Model}' updated successfully!";
            return RedirectToAction(nameof(FridgeUnit));
        }


        // VIEW FRIDGE DETAILS
        public async Task<IActionResult> FridgeDetails(int id)
        {
            var fridge = await _context.Fridges
                .Include(f => f.Inventory)
                .Include(f => f.Supplier)
                .Include(f => f.WarehouseStocks)
                    .ThenInclude(ws => ws.Warehouse)
                .FirstOrDefaultAsync(f => f.FridgeId == id && (f.isDeleted == null || f.isDeleted == false));

            if (fridge == null)
                return NotFound();

            // Get warehouse name (if fridge is in stock and not allocated)
            var warehouseName = fridge.WarehouseStocks.FirstOrDefault()?.Warehouse?.Name ?? "Not Assigned";

            var model = new FridgeDetailsViewModel
            {
                FridgeId = fridge.FridgeId,
                Model = fridge.Model,
                FridgeColor = fridge.FridgeColor,
                SerialNumber = fridge.SerialNumber,
                FridgeDescription = fridge.Specification,
                FridgeImage = fridge.FridgeImage,
                WarehouseName = warehouseName,
                SupplierName = fridge.Supplier?.SupplierName ?? "Unknown Supplier",
                StockQuantity = fridge.StockQuantity,
                IsAvailable = fridge.IsAvailable ?? false,
                PurchaseDate = fridge.PurchaseDate,
                WarrantyExpiryDate = fridge.WarrantyExpiryDate,
                DeliveryDate = fridge.DeliveryDate,
                Category = fridge.Category.ToString(),
                CreatedDate = fridge.PurchaseDate,
                UpdatedDate = fridge.DeliveryDate
            };

            return View("FridgeDetails", model);
        }

        // SCRAP FRIDGE 
        [HttpGet]
        public async Task<IActionResult> ScrapFridge(int id)
        {
            var fridge = await _context.Fridges
                .Include(f => f.Inventory)
                .Include(f => f.WarehouseStocks)
                    .ThenInclude(ws => ws.Warehouse)
                .FirstOrDefaultAsync(f => f.FridgeId == id && (f.isDeleted == null || f.isDeleted == false));

            if (fridge == null)
                return NotFound();

            var warehouseName = fridge.WarehouseStocks.FirstOrDefault()?.Warehouse?.Name ?? "Not Assigned";

            var model = new ScrapFridgeViewModel
            {
                FridgeId = fridge.FridgeId,
                Model = fridge.Model,
                QuantityInStock = fridge.Inventory?.QuantityInStock ?? 1,
                WarehouseName = warehouseName
            };

            return View("ScrapFridge", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScrapFridge(ScrapFridgeViewModel model)
        {
            if (!ModelState.IsValid)
                return View("ScrapFridge", model);

            var fridge = await _context.Fridges
                .Include(f => f.Inventory)
                .Include(f => f.WarehouseStocks)
                .FirstOrDefaultAsync(f => f.FridgeId == model.FridgeId && (f.isDeleted == null || f.isDeleted == false));

            if (fridge == null)
                return NotFound();

            // Get logged-in user (ASP.NET Identity)
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            // Decrease inventory stock if exists
            if (fridge.Inventory != null)
            {
                fridge.Inventory.QuantityInStock = Math.Max(fridge.Inventory.QuantityInStock - 1, 0);
                fridge.Inventory.LastUpdate = DateTime.UtcNow;
            }

            // Mark as scrapped (no Status table anymore)
            fridge.IsAvailable = false;
            fridge.isDeleted = true;

            // Record history
            var history = new FridgeStatusHistory
            {
                InventoryId = fridge.InventoryId ?? 0,
                LastUpdate = DateTime.UtcNow,
                Note = $"Fridge scrapped. Reason: {model.Reason}",
                EventId = (await _context.EventTypes.FirstOrDefaultAsync(e => e.Event == "Scrapped"))?.EventId ?? 0,
                EmployeeId = 0,
                LocationId = fridge.CustomerLocationId ?? 0
            };

            _context.FridgeStatusHistory.Add(history);

            // Notify ALL Inventory Liaisons
            var inventoryLiaisons = await _context.Users
                .Where(u => _userManager.IsInRoleAsync(u, "Inventory Liaison").Result)
                .ToListAsync();

            foreach (var liaison in inventoryLiaisons)
            {
                var notification = new EmployeeNotification
                {
                    EmployeeId = 0,
                    Message = $"Fridge '{fridge.Model}' (Serial: {fridge.SerialNumber}) was scrapped. Reason: {model.Reason}",
                    SentDate = DateTime.UtcNow
                };
                _context.EmployeesNotifications.Add(notification);
            }

            // Update the fridge record
            _context.Update(fridge);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Fridge '{fridge.Model}' scrapped successfully!";
            return RedirectToAction(nameof(Inventory));
        }

        // VIEW FRIDGE HISTORY FOR SINGLE FRIDGE UNIT
        public async Task<IActionResult> FridgeHistory(int fridgeId)
        {
            var fridge = await _context.Fridges
                .Include(f => f.Inventory)
                .Include(f => f.WarehouseStocks)
                    .ThenInclude(ws => ws.Warehouse)
                .Include(f => f.FridgeStatusHistory)
                    .ThenInclude(h => h.Employee)
                .Include(f => f.FridgeStatusHistory)
                    .ThenInclude(h => h.Location)
                .FirstOrDefaultAsync(f => f.FridgeId == fridgeId && (f.isDeleted == null || f.isDeleted == false));

            if (fridge == null)
                return NotFound();

            // Determine status based on availability
            string status = fridge.isDeleted == true
                ? "Scrapped"
                : fridge.IsAvailable == true
                    ? "Available"
                    : "Unavailable";

            // Get warehouse info (if any)
            var warehouseStock = fridge.WarehouseStocks.FirstOrDefault();
            string warehouseName = warehouseStock?.Warehouse?.Name ?? "N/A";
            int stockQty = warehouseStock?.Quantity ?? 0;

            var model = new FridgeHistoryViewModel
            {
                FridgeId = fridge.FridgeId,
                FridgeName = fridge.Model, // or fridge.FridgeName if exists
                Model = fridge.Model,
                SerialNumber = fridge.SerialNumber,
                Category = fridge.Category.ToString(),
                FridgeColor = fridge.FridgeColor,
                Status = status,
                WarehouseName = warehouseName,
                
                StatusHistory = fridge.FridgeStatusHistory.Select(h => new FridgeStatusHistoryViewModel
                {
                    LastUpdate = h.LastUpdate,
                    Note = h.Note,
                    EventType = h.EventType.Event,
                    EmployeeName = h.Employee != null ? h.Employee.FirstName + " " + h.Employee.LastName : "—",
                    Location = h.Location != null ? h.Location.BranchName : "—"
                }).OrderByDescending(h => h.LastUpdate).ToList()
            };

            if (!model.StatusHistory.Any())
            {
                TempData["Info"] = "This fridge has no history yet.";
            }

            return View("FridgeHistory", model);
        }

        // LIST PURCHASE REQUESTS
        public async Task<IActionResult> PurchaseRequests()
        {
            var requests = await _context.PurchaseRequests
                .Include(r => r.Inventory)
                .Include(r => r.RequestedBy)
                .ToListAsync();

            var model = requests.Select(r => new PurchaseRequestViewModel
            {
                PurchaseRequestId = r.RequestId,
                InventoryId = r.InventoryId,
                FridgeModel = r.Inventory?.Model,
                Quantity = r.QuantityRequested,
                ReasonForRequest = r.ReasonForRequest,
                RequestedByName = r.RequestedBy != null
                    ? $"{r.RequestedBy.FirstName} {r.RequestedBy.LastName}"
                    : "Unknown",
                RequestedDate = r.RequestedDate,
                Status = r.RequestStatus
            }).ToList();

            return View("PurchaseRequest", model);
        }

        // CREATE PURCHASE REQUEST - GET
        [HttpGet]
        public async Task<IActionResult> CreatePurchaseRequest()
        {
            var model = new PurchaseRequestViewModel
            {
                FridgeModels = await _context.FridgesInventory
        .Select(f => new SelectListItem
        {
            Value = f.InventoryId.ToString(),
            Text = f.Model
        }).ToListAsync(),
                Statuses = Enum.GetValues(typeof(RequestStatus))
        .Cast<RequestStatus>()
        .Select(s => new SelectListItem
        {
            Value = ((int)s).ToString(),
            Text = s.ToString()
        }).ToList(),
                Status = RequestStatus.Pending // default value
            };

            return View("CreatePurchaseRequest", model);
        }

        // CREATE PURCHASE REQUEST - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePurchaseRequest(PurchaseRequestViewModel model)
        {
            // 1. Set the date on the ViewModel object (for consistency/debugging)
            model.RequestedDate = DateTime.UtcNow;

            // 💡 CRITICAL FIX: Remove the validation error created by the model binder for RequestedDate.
            // The model binder fails when no value is submitted for a DateTime property.
            ModelState.Remove(nameof(model.RequestedDate));

            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns on validation failure before returning the view
                model.FridgeModels = await _context.FridgesInventory
                    .Select(f => new SelectListItem { Value = f.InventoryId.ToString(), Text = f.Model })
                    .ToListAsync();

                // The TempData error will now appear alongside the specific field errors.
                TempData["Error"] = "Validation failed! Please correct the errors below.";
                return View("CreatePurchaseRequest", model);
            }

            // 2. Get logged-in employee from Identity
            var userId = _userManager.GetUserId(User);
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.ApplicationUserId == userId);

            // CRITICAL: If the user is logged in but has no Employee record, stop here.
            if (employee == null)
            {
                TempData["Error"] = "Error: Cannot find a linked Employee account for the logged-in user. Request failed.";
                // Ensure you repopulate dropdowns here
                model.FridgeModels = await _context.FridgesInventory
                    .Select(f => new SelectListItem { Value = f.InventoryId.ToString(), Text = f.Model })
                    .ToListAsync();
                return View("CreatePurchaseRequest", model);
            }

            try
            {
                // 3. Create Entity with all required fields
                var request = new PurchaseRequest
                {
                    // Use ?? 0 only if InventoryId is nullable in the entity; otherwise, model.InventoryId.Value
                    InventoryId = model.InventoryId.Value,
                    QuantityRequested = model.Quantity,
                    ReasonForRequest = model.ReasonForRequest,
                    RequestedById = employee.EmployeeId,
                    RequestedDate = DateTime.UtcNow,  // always now (used for entity save)
                    RequestStatus = RequestStatus.Pending, // <-- force pending
                    EmployeeId = employee.EmployeeId
                };

                _context.PurchaseRequests.Add(request);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Purchase request submitted successfully!";
                return RedirectToAction(nameof(PurchaseRequests));
            }
            catch (Exception ex)
            {
                // Log the exact database error for debugging
                _logger.LogError(ex, "DATABASE SAVE FAILED: Purchase Request could not be saved.");
                TempData["Error"] = "An error occurred during save. Check logs/debug for the exact database error.";

                // Ensure you repopulate dropdowns here before returning
                model.FridgeModels = await _context.FridgesInventory
                    .Select(f => new SelectListItem { Value = f.InventoryId.ToString(), Text = f.Model })
                    .ToListAsync();
                return View("CreatePurchaseRequest", model);
            }
        }

        // EDIT PURCHASE REQUEST - GET
        [HttpGet]
        public async Task<IActionResult> EditPurchaseRequest(int id)
        {
            var request = await _context.PurchaseRequests
                .Include(r => r.Inventory)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null) return NotFound();

            var model = new PurchaseRequestViewModel
            {
                PurchaseRequestId = request.RequestId,
                InventoryId = request.InventoryId,
                Quantity = request.QuantityRequested,
                ReasonForRequest = request.ReasonForRequest,
                Status = request.RequestStatus,
                FridgeModels = (await _context.FridgesInventory.ToListAsync())
                    .Select(f => new SelectListItem
                    {
                        Value = f.InventoryId.ToString(),
                        Text = f.Model
                    })
                    .ToList(),
                Statuses = Enum.GetValues(typeof(RequestStatus))
                    .Cast<RequestStatus>()
                    .Select(s => new SelectListItem
                    {
                        Value = ((int)s).ToString(),
                        Text = s.ToString()
                    })
                    .ToList()
            };

            return View("EditPurchaseRequest", model);
        }

        // EDIT PURCHASE REQUEST - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPurchaseRequest(PurchaseRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.FridgeModels = (await _context.FridgesInventory.ToListAsync())
                    .Select(f => new SelectListItem
                    {
                        Value = f.InventoryId.ToString(),
                        Text = f.Model
                    })
                    .ToList();

                model.Statuses = Enum.GetValues(typeof(RequestStatus))
                    .Cast<RequestStatus>()
                    .Select(s => new SelectListItem
                    {
                        Value = ((int)s).ToString(),
                        Text = s.ToString()
                    })
                    .ToList();

                return View("EditPurchaseRequest", model);
            }

            var request = await _context.PurchaseRequests.FindAsync(model.PurchaseRequestId);
            if (request == null) return NotFound();

            request.InventoryId = (int)model.InventoryId;
            request.QuantityRequested = model.Quantity;
            request.ReasonForRequest = model.ReasonForRequest;
            request.RequestStatus = model.Status;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Purchase request updated successfully!";
            return RedirectToAction(nameof(PurchaseRequests));
        }

        // DELETE PURCHASE REQUEST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePurchaseRequest(int id)
        {
            var request = await _context.PurchaseRequests.FindAsync(id);
            if (request != null)
            {
                _context.PurchaseRequests.Remove(request);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Purchase request deleted successfully!";
            }

            return RedirectToAction(nameof(PurchaseRequests));
        }

        // STOCK ALERT - LIST OF LOW STOCK ITEMS
        public async Task<IActionResult> StockAlert()
        {
            // Get the currently logged-in Inventory Liaison
            var userId = _userManager.GetUserId(User);
            var liaison = await _context.Employees
                .FirstOrDefaultAsync(e => e.ApplicationUserId == userId);

            if (liaison == null)
                return Unauthorized();

            // Low stock alerts (<= 5 units)
            var lowStockList = await _context.FridgesInventory
                .Where(f => f.InventoryLiasonEmployeeId == liaison.EmployeeId && f.QuantityInStock <= 5)
                .Select(f => new StockAlertViewModel
                {
                    InventoryId = f.InventoryId,
                    Model = f.Model,
                    Category = f.Category.ToString(),
                    QuantityInStock = f.QuantityInStock,
                    SupplierName = f.Supplier != null ? f.Supplier.SupplierName : "Unknown",
                    LastUpdate = f.LastUpdate
                })
                .ToListAsync();

            return View(lowStockList);
        }
    }
}