using FridgeSystem.Data;
using FridgeSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FridgeSystem.Models.ViewModels;

namespace FridgeSystem.Controllers
{
    [Authorize(Roles = "Customer Liaison")]
    public class CustomerLiaisonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerLiaisonController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Helper: get current Employee
        private async Task<Employee> GetCurrentEmployeeAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
        }


        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var employee = await GetCurrentEmployeeAsync();

            if (employee == null)
            {
                // Employee record missing for this user, redirect safely to Home
                TempData["ErrorMessage"] = "Employee profile not found. Please contact admin.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Get fridge allocations by this liaison
            var allocations = await _context.FridgesAllocations
                .Include(fa => fa.Fridge)
                .Include(fa => fa.CustomerLocation)
                .Where(fa => fa.AllocatedById == employee.EmployeeId)
                .ToListAsync();

            // Get pending fridge requests
            var requests = await _context.FridgeRequests
                .Include(r => r.Customer)
                .Where(r => r.ApprovedDate == default(DateTime))
                .ToListAsync();

            // Get only unread notifications for dashboard display
            var notifications = await _context.EmployeesNotifications
                .Where(n => n.EmployeeId == employee.EmployeeId && !n.IsRead)
                .OrderByDescending(n => n.SentDate)
                .ToListAsync();

            // Build dashboard ViewModel
            var dashboardVM = new CustLiaisonDashboardViewModel
            {
                EmployeeName = employee.ApplicationUser?.FirstName ?? "Employee",
                TotalCustomers = await _context.Customers.CountAsync(c => c.ProfileActive),
                ActiveAllocations = allocations.Count,
                PendingRequests = requests.Count,
                UnreadNotifications = notifications.Count, // Only unread ones now

                // Show only unread + most recent 5
                RecentNotifications = notifications
                    .Take(5)
                    .Select(n => new EmployeeNotificationViewModel
                    {
                        NotificationId = n.NotificationId,
                        Message = n.Message,
                        SentDate = n.SentDate,
                        IsRead = n.IsRead
                    })
                    .ToList(),

                // Latest 5 fridge allocations
                RecentAllocations = allocations
                    .OrderByDescending(a => a.AllocatedDate)
                    .Take(5)
                    .Select(a => $"{a.Fridge?.Model} ({a.Fridge?.SerialNumber}) to {a.CustomerLocation?.BranchName}")
                    .ToList()
            };

            return View(dashboardVM);
        }



        // GET: Employee Notifications
        public IActionResult EmployeeNotifications()
        {
            
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var employee = _context.Employees.FirstOrDefault(e => e.ApplicationUserId == userId);
            if (employee == null) return Unauthorized();

            var notifications = _context.EmployeesNotifications
                .Where(n => n.EmployeeId == employee.EmployeeId)
                .OrderByDescending(n => n.SentDate)
                .ToList();

            var model = notifications.Select(n => new EmployeeNotificationViewModel
            {
                NotificationId = n.NotificationId,
                Message = n.Message,
                SentDate = n.SentDate,
                IsRead = n.IsRead
            }).ToList();

            return View("EmployeeNotifications", model);
        }

        // POST: Mark single notification as read
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAsRead(int id)
        {
            var notification = _context.EmployeesNotifications.Find(id);
            if (notification == null) return NotFound();

            notification.IsRead = true;
            _context.Update(notification);
            _context.SaveChanges();

            return RedirectToAction(nameof(EmployeeNotifications));
        }

        // POST: Mark all notifications as read
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAllAsRead()
        {
            // Get current user
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            // Find employee
            var employee = _context.Employees.FirstOrDefault(e => e.ApplicationUserId == userId);
            if (employee == null) return Unauthorized();

            // Get all unread notifications for that employee
            var unreadNotifications = _context.EmployeesNotifications
                .Where(n => n.EmployeeId == employee.EmployeeId && !n.IsRead)
                .ToList();

            foreach (var n in unreadNotifications)
                n.IsRead = true;

            _context.UpdateRange(unreadNotifications);
            _context.SaveChanges();

            return RedirectToAction(nameof(EmployeeNotifications));
        }


        // CUSTOMERS (BUSINESSES)
        public async Task<IActionResult> Customers(string? searchText, string? statusFilter)
        {
            var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);

            // Auto-deactivate inactive profiles
            var inactiveCustomers = await _context.Customers
                .Include(c => c.CustomerLocations)
                .Where(c => c.ProfileActive &&
                    ((!c.LastLoginDate.HasValue || c.LastLoginDate < threeMonthsAgo) &&
                     !_context.FridgesAllocations.Any(fa => fa.BusinessName.BusinessId == c.BusinessId && fa.AllocatedDate >= threeMonthsAgo) &&
                     !_context.ServiceSchedules.Any(ss => ss.LocationId == c.BusinessId && ss.StartDate >= threeMonthsAgo)))
                .ToListAsync();

            foreach (var cust in inactiveCustomers)
            {
                cust.ProfileActive = false;
                _context.Update(cust);
            }
            await _context.SaveChangesAsync();

            // Query with search/filter
            var query = _context.Customers
                .Include(c => c.CustomerLocations)
                .Include(c => c.BusinessType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();
                query = query.Where(c =>
                    c.BusinessId.ToString().Contains(searchText) ||
                    c.CustomerLocations.Any(l =>
                        l.CustomerLocationId.ToString().Contains(searchText) ||
                        l.BranchName.Contains(searchText) ||
                        l.City.Contains(searchText) ||
                        l.Province.Contains(searchText)
                    ) ||
                    c.BusinessName.Contains(searchText)
                );
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = statusFilter.Equals("active", StringComparison.OrdinalIgnoreCase)
                    ? query.Where(c => c.ProfileActive)
                    : query.Where(c => !c.ProfileActive);
            }

            var customers = await query.OrderBy(c => c.BusinessName).ToListAsync();

            var model = customers.Select(c => new CustomerViewModel
            {
                BusinessId = c.BusinessId,
                BusinessName = c.BusinessName,
                Email = c.Email,
                ContactPersonName = c.ContactPersonName,
                Telephone = c.Telephone,
                BillingAddress = c.BillingAddress,
                BusinessTypeName = c.BusinessType?.TypeName ?? "", 
                ProfileActive = c.ProfileActive,
                DateCreated = c.DateCreated,
                TotalLocations = c.CustomerLocations?.Count ?? 0
            }).ToList();

            ViewBag.SearchText = searchText;
            ViewBag.StatusFilter = statusFilter;

            return View("Customers", model);
        }



        // Add Customer
        [HttpGet]
        public async Task<IActionResult> AddCustomer()
        {
            var businessTypes = await _context.BusinessTypes
                .OrderBy(bt => bt.TypeName)
                .Select(bt => new SelectListItem
                {
                    Value = bt.BusinessTypeId.ToString(),
                    Text = bt.TypeName
                })
                .ToListAsync();

            var model = new CustomerViewModel
            {
                BusinessTypeList = businessTypes
            };

            return View("AddCustomer", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomer(CustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                
                model.BusinessTypeList = await _context.BusinessTypes
                    .Select(bt => new SelectListItem
                    {
                        Value = bt.BusinessTypeId.ToString(),
                        Text = bt.TypeName
                    }).ToListAsync();

                return View("AddCustomer", model);
            }

            
            var password = string.IsNullOrWhiteSpace(model.Password)
                ? Guid.NewGuid().ToString().Substring(0, 8) 
                : model.Password;

            var userAccount = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.BusinessName,
                LastName = "",
                IsActive = true
            };

            var result = await _userManager.CreateAsync(userAccount, password);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Failed to create user account: " +
                                     string.Join(", ", result.Errors.Select(e => e.Description));

                model.BusinessTypeList = await _context.BusinessTypes
                    .Select(bt => new SelectListItem
                    {
                        Value = bt.BusinessTypeId.ToString(),
                        Text = bt.TypeName
                    }).ToListAsync();

                return View("AddCustomer", model);
            }

            
            var newCustomer = new Customer
            {
                BusinessName = model.BusinessName,
                Email = model.Email,
                ContactPersonName = model.ContactPersonName,
                Telephone = model.Telephone,
                BillingAddress = model.BillingAddress,
                BusinessTypeId = model.BusinessTypeId,
                DateCreated = DateTime.UtcNow,
                ProfileActive = true,
                ApplicationUserId = userAccount.Id 
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            
            TempData["OneTimePassword"] = password;
            TempData["Success"] = $"Customer '{model.BusinessName}' added successfully. Temporary Password: {password}";

            return RedirectToAction(nameof(Customers));
        }


        // Edit Customer
        [HttpGet]
        public async Task<IActionResult> EditCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.BusinessType)
                .FirstOrDefaultAsync(c => c.BusinessId == id);

            if (customer == null)
                return NotFound();

            var model = new CustomerViewModel
            {
                BusinessId = customer.BusinessId,
                BusinessName = customer.BusinessName,
                Email = customer.Email,
                ContactPersonName = customer.ContactPersonName,
                Telephone = customer.Telephone,
                BillingAddress = customer.BillingAddress,
                BusinessTypeId = customer.BusinessTypeId,
                ProfileActive = customer.ProfileActive,
                BusinessTypeList = await _context.BusinessTypes
                    .Select(bt => new SelectListItem
                    {
                        Value = bt.BusinessTypeId.ToString(),
                        Text = bt.TypeName
                    }).ToListAsync()
            };

            return View("EditCustomer", model);
        }

        // POST: Edit Customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(int id, CustomerViewModel model)
        {
            if (id != model.BusinessId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // repopulate dropdown if validation fails
                model.BusinessTypeList = await _context.BusinessTypes
                    .Select(bt => new SelectListItem
                    {
                        Value = bt.BusinessTypeId.ToString(),
                        Text = bt.TypeName
                    }).ToListAsync();

                return View("EditCustomer", model);
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.BusinessId == id);

            if (customer == null)
                return NotFound();

            customer.BusinessName = model.BusinessName;
            customer.Email = model.Email;
            customer.ContactPersonName = model.ContactPersonName;
            customer.Telephone = model.Telephone;
            customer.BillingAddress = model.BillingAddress;
            customer.ProfileActive = model.ProfileActive;
            customer.BusinessTypeId = model.BusinessTypeId;

            _context.Update(customer);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Customer updated successfully.";
            return RedirectToAction(nameof(Customers));
        }


        // Delete Customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.CustomerLocations)
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(c => c.BusinessId == id);

            if (customer != null)
            {
                // Remove related locations
                _context.CustomerLocations.RemoveRange(customer.CustomerLocations);

                // Remove linked Identity user
                if (customer.ApplicationUser != null)
                {
                    await _userManager.DeleteAsync(customer.ApplicationUser);
                }

                // Remove customer
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Customer deleted successfully.";
            }

            return RedirectToAction(nameof(Customers));
        }


        // CUSTOMER LOCATIONS
        public async Task<IActionResult> CustomerLocations(int businessId)
        {
            var customer = await _context.Customers
                .Include(c => c.CustomerLocations)
                .FirstOrDefaultAsync(c => c.BusinessId == businessId);

            if (customer == null)
                return NotFound();

            var model = customer.CustomerLocations.Select(l => new CustomerLocationViewModel
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

            return View("CustomerLocations", model);
        }

        // ADD LOCATION (GET)
        [HttpGet]
        public IActionResult AddLocation(int businessId)
        {
            var customerName = _context.Customers
                .Where(c => c.BusinessId == businessId)
                .Select(c => c.BusinessName)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(customerName))
                return NotFound("Customer not found.");

            ViewBag.BusinessId = businessId;
            ViewBag.CustomerName = customerName;

            var model = new List<CustomerLocationViewModel>
            {
               new CustomerLocationViewModel { BusinessId = businessId, IsActive = true }
            };

            return View("AddCustomerLocation", model);
        }

        // ADD LOCATION (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLocation(List<CustomerLocationViewModel> models)
        {
            if (models == null || !models.Any())
            {
                TempData["Error"] = "No branches submitted!";
                return RedirectToAction(nameof(CustomerLocations));
            }

            if (!ModelState.IsValid)
            {
                int businessId = models.FirstOrDefault()?.BusinessId ?? 0;
                ViewBag.BusinessId = businessId;
                ViewBag.CustomerName = await _context.Customers
                    .Where(c => c.BusinessId == businessId)
                    .Select(c => c.BusinessName)
                    .FirstOrDefaultAsync();

                TempData["Error"] = "Validation failed!";
                return View("AddCustomerLocation", models);
            }

            var newLocations = models.Select(m => new CustomerLocation
            {
                BusinessId = m.BusinessId,
                BranchName = m.BranchName,
                AddressLine1 = m.AddressLine1,
                AddressLine2 = m.AddressLine2,
                City = m.City,
                Province = m.Province,
                Postcode = m.Postcode,
                Telephone = m.Telephone,
                ContactPerson = m.ContactPerson,
                IsActive = m.IsActive
            }).ToList();

            await _context.CustomerLocations.AddRangeAsync(newLocations);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{newLocations.Count} branch(es) added successfully.";

            return RedirectToAction(nameof(CustomerLocations), new { businessId = models.First().BusinessId });
        }


        // EDIT LOCATION (GET)
        [HttpGet]
        public async Task<IActionResult> EditLocation(int id)
        {
            var location = await _context.CustomerLocations.FindAsync(id);
            if (location == null)
                return NotFound();

            ViewBag.BusinessId = location.BusinessId;
            ViewBag.CustomerName = await _context.Customers
                .Where(c => c.BusinessId == location.BusinessId)
                .Select(c => c.BusinessName)
                .FirstOrDefaultAsync();

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

            return View("EditCustomerLocation", model);
        }

        // EDIT LOCATION (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLocation(int id, CustomerLocationViewModel model)
        {
            if (id != model.LocationId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.BusinessId = model.BusinessId;
                ViewBag.CustomerName = await _context.Customers
                    .Where(c => c.BusinessId == model.BusinessId)
                    .Select(c => c.BusinessName)
                    .FirstOrDefaultAsync();

                return View("EditCustomerLocation", model);
            }

            var location = await _context.CustomerLocations.FindAsync(id);
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

            TempData["Success"] = "Location updated successfully.";
            return RedirectToAction(nameof(CustomerLocations), new { businessId = model.BusinessId });
        }


        // Fridge Allocations       
        public async Task<IActionResult> Allocations()
        {
            var allocations = await _context.FridgesAllocations
                .Include(a => a.BusinessName)
                .Include(a => a.CustomerLocation)
                .Include(a => a.Fridge)
                .ToListAsync();

            var model = allocations.Select(a => new FridgeAllocationViewModel
            {
                AllocationId = a.AllocationId,
                BusinessId = a.BusinessName?.BusinessId ?? 0,
                BusinessName = a.BusinessName?.BusinessName ?? "",
                LocationId = a.CustomerLocation?.CustomerLocationId ?? 0,
                BranchName = a.CustomerLocation?.BranchName ?? "",
                FridgeId = a.FridgeId,
                Model = a.Fridge?.Model ?? "",
                SerialNumber = a.Fridge?.SerialNumber ?? "",
                AllocatedById = a.AllocatedById,
                AllocatedDate = a.AllocatedDate
            }).ToList();

            return View("FridgeAllocations", model);
        }

        // GET: Create Allocation
        [HttpGet]
        public async Task<IActionResult> CreateAllocation(int? requestId = null)
        {
            FridgeAllocationViewModel model;

            if (requestId.HasValue)
            {
                var request = await _context.FridgeRequests
                    .Include(r => r.Customer)
                    .Include(r => r.CustomerLocation)
                    .Include(r => r.FridgeRequestItems)
                        .ThenInclude(f => f.Fridge)
                            .ThenInclude(fr => fr.Inventory)
                    .FirstOrDefaultAsync(r => r.RequestId == requestId.Value);

                if (request == null) return NotFound();

                var preselectedFridges = request.FridgeRequestItems.Select(f => new FridgeRequestItemViewModel
                {
                    SelectedFridgeId = f.FridgeId,
                    InventoryId = f.Fridge.Inventory.InventoryId,
                    Model = f.Fridge.Inventory.Model,
                    Quantity = f.Quantity
                }).ToList();

                model = new FridgeAllocationViewModel
                {
                    BusinessId = request.BusinessId,
                    LocationId = request.CustomerLocationId,
                    Customers = new List<SelectListItem>
                    {
                        new SelectListItem { Value = request.BusinessId.ToString(), Text = request.Customer.BusinessName }
                    },
                    Locations = new List<SelectListItem>
                    {
                        new SelectListItem { Value = request.CustomerLocationId.ToString(), Text = request.CustomerLocation.BranchName }
                    },
                    RequestedFridges = preselectedFridges
                };
            }
            else
            {
                var customers = await _context.Customers
                    .Where(c => c.ProfileActive && c.isDeleted != true)
                    .OrderBy(c => c.BusinessName)
                    .ToListAsync();

                var availableFridges = await _context.Fridges
                    .Where(f => f.IsAvailable == true && f.isDeleted != true)
                    .Include(f => f.Inventory)
                    .ToListAsync();

                model = new FridgeAllocationViewModel
                {
                    Customers = customers.Select(c => new SelectListItem
                    {
                        Value = c.BusinessId.ToString(),
                        Text = c.BusinessName
                    }),
                    Fridges = availableFridges.Select(f => new SelectListItem
                    {
                        Value = f.FridgeId.ToString(),
                        Text = $"{f.Inventory.Model} - {f.SerialNumber}"
                    }),
                    Locations = new List<SelectListItem>(),
                    RequestedFridges = new List<FridgeRequestItemViewModel>()
                };
            }

            return View("CreateAllocation", model);
        }

        // POST: Create Allocation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAllocation(FridgeAllocationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Customers = (await _context.Customers
                    .Where(c => c.ProfileActive && c.isDeleted != true)
                    .OrderBy(c => c.BusinessName)
                    .ToListAsync())
                    .Select(c => new SelectListItem { Value = c.BusinessId.ToString(), Text = c.BusinessName });

                model.Fridges = (await _context.Fridges
                    .Where(f => f.IsAvailable == true && f.isDeleted != true)
                    .Include(f => f.Inventory)
                    .ToListAsync())
                    .Select(f => new SelectListItem { Value = f.FridgeId.ToString(), Text = $"{f.Inventory.Model} - {f.SerialNumber}" });

                if (model.BusinessId > 0)
                {
                    model.Locations = (await _context.CustomerLocations
                        .Where(l => l.BusinessId == model.BusinessId && l.IsActive)
                        .ToListAsync())
                        .Select(l => new SelectListItem { Value = l.CustomerLocationId.ToString(), Text = l.BranchName });
                }

                return View("CreateAllocation", model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var liaison = await _context.Employees.FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
            if (liaison == null)
            {
                TempData["Error"] = "You are not authorized to allocate fridges.";
                return RedirectToAction(nameof(CreateAllocation));
            }

            var customer = await _context.Customers.FindAsync(model.BusinessId);
            var location = await _context.CustomerLocations.FindAsync(model.LocationId);

            if (customer == null || location == null)
            {
                TempData["Error"] = "Invalid Customer or Location selection.";
                return RedirectToAction(nameof(CreateAllocation));
            }

            foreach (var fridgeItem in model.RequestedFridges)
            {
                var fridge = await _context.Fridges.FindAsync(fridgeItem.SelectedFridgeId);
                if (fridge == null || fridge.IsAvailable != true) continue;

                var allocation = new FridgeAllocation
                {
                    BusinessName = customer,
                    FridgeId = fridge.FridgeId,
                    CustomerLocation = location,
                    AllocatedById = liaison.EmployeeId,
                    AllocatedDate = DateTime.Now
                };

                fridge.IsAvailable = false;

                var inventory = await _context.FridgesInventory.FindAsync(fridge.InventoryId);
                if (inventory != null && inventory.QuantityInStock > 0)
                {
                    inventory.QuantityInStock -= 1;
                    inventory.LastUpdate = DateTime.UtcNow;
                }

                _context.FridgesAllocations.Add(allocation);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Fridge allocation(s) created successfully.";
            return RedirectToAction(nameof(Allocations));
        }

        // EDIT ALLOCATION (GET)
        [HttpGet]
        public async Task<IActionResult> EditAllocation(int id)
        {
            var allocation = await _context.FridgesAllocations
                .Include(a => a.BusinessName)
                .Include(a => a.CustomerLocation)
                .Include(a => a.Fridge)
                .FirstOrDefaultAsync(a => a.AllocationId == id);

            if (allocation == null) return NotFound();

            var model = new FridgeAllocationViewModel
            {
                AllocationId = allocation.AllocationId,
                BusinessId = allocation.BusinessName?.BusinessId ?? 0,
                LocationId = allocation.CustomerLocation?.CustomerLocationId ?? 0,
                FridgeId = allocation.FridgeId,
                BusinessName = allocation.BusinessName?.BusinessName ?? "",
                BranchName = allocation.CustomerLocation?.BranchName ?? "",
                Model = allocation.Fridge?.Model ?? "",
                SerialNumber = allocation.Fridge?.SerialNumber ?? "",
                AllocatedById = allocation.AllocatedById,
                AllocatedDate = allocation.AllocatedDate,

                Customers = (await _context.Customers
                    .Where(c => c.ProfileActive && c.isDeleted != true)
                    .OrderBy(c => c.BusinessName)
                    .ToListAsync())
                    .Select(c => new SelectListItem { Value = c.BusinessId.ToString(), Text = c.BusinessName }),

                Fridges = (await _context.Fridges
                    .Where(f => f.isDeleted != true)
                    .OrderBy(f => f.Model)
                    .ToListAsync())
                    .Select(f => new SelectListItem { Value = f.FridgeId.ToString(), Text = $"{f.Model} - {f.SerialNumber}" }),

                Locations = (await _context.CustomerLocations
                    .Where(l => l.BusinessId == allocation.BusinessName.BusinessId && l.IsActive)
                    .ToListAsync())
                    .Select(l => new SelectListItem { Value = l.CustomerLocationId.ToString(), Text = l.BranchName })
            };

            return View("EditAllocation", model);
        }

        // EDIT ALLOCATION (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAllocation(int id, FridgeAllocationViewModel model)
        {
            if (id != model.AllocationId) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var allocation = await _context.FridgesAllocations
                .Include(a => a.BusinessName)
                .Include(a => a.CustomerLocation)
                .Include(a => a.Fridge)
                .FirstOrDefaultAsync(a => a.AllocationId == id);

            if (allocation == null) return NotFound();

            var customer = await _context.Customers.FindAsync(model.BusinessId);
            var fridge = await _context.Fridges.FindAsync(model.FridgeId);
            var location = await _context.CustomerLocations.FindAsync(model.LocationId);

            if (customer == null || fridge == null || location == null)
            {
                TempData["Error"] = "Invalid Customer, Fridge, or Location selection.";
                return View(model);
            }

            allocation.BusinessName = customer;
            allocation.FridgeId = fridge.FridgeId;
            allocation.CustomerLocation = location;

            _context.Update(allocation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Allocation updated successfully.";
            return RedirectToAction(nameof(Allocations));
        }

        // CANCEL ALLOCATION
        public async Task<IActionResult> CancelAllocation(int id)
        {
            var allocation = await _context.FridgesAllocations.FindAsync(id);
            if (allocation == null) return NotFound();

            var fridge = await _context.Fridges.FindAsync(allocation.FridgeId);
            if (fridge != null) fridge.IsAvailable = true;

            _context.FridgesAllocations.Remove(allocation);
            await _context.SaveChangesAsync();

            TempData["Info"] = "Allocation cancelled successfully.";
            return RedirectToAction(nameof(Allocations));
        }

        // AJAX: Get Customer Locations
        [HttpGet]
        public async Task<IActionResult> GetCustomerLocations(int customerId)
        {
            var locations = await _context.CustomerLocations
                .Where(l => l.BusinessId == customerId && l.IsActive)
                .Select(l => new
                {
                    id = l.CustomerLocationId,
                    text = l.BranchName
                })
                .ToListAsync();

            return Json(locations);
        }


        // FRIDGE REQUESTS - CustomerLiaison
        public async Task<IActionResult> FridgeRequests()
        {
            var requests = await _context.FridgeRequests
                .Include(r => r.Customer)
                .Include(r => r.CustomerLocation)
                .Include(r => r.FridgeRequestItems)
                    .ThenInclude(f => f.Fridge)
                        .ThenInclude(fr => fr.Inventory)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            var model = requests.Select(r => new FridgeRequestViewModel
            {
                RequestId = r.RequestId,
                ContactNumber = r.ContactNumber,
                SpecialInstructions = r.SpecialInstructions,
                PreferredPickup = r.PreferredPickup,
                BusinessId = r.BusinessId,
                BusinessName = r.Customer.BusinessName,
                CustomerLocationId = r.CustomerLocationId,
                BranchName = r.CustomerLocation.BranchName,
                RequestedFridges = r.FridgeRequestItems.Select(f => new FridgeRequestItemViewModel
                {
                    InventoryId = f.Fridge.Inventory.InventoryId,
                    Model = f.Fridge.Inventory.Model,
                    Quantity = f.Quantity
                }).ToList()
            }).ToList();

            return View("FridgeRequests", model);
        }

        // GET: Approve Request
        [HttpGet]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var request = await _context.FridgeRequests
                .Include(r => r.Customer)
                .Include(r => r.CustomerLocation)
                .Include(r => r.FridgeRequestItems)
                    .ThenInclude(f => f.Fridge)
                        .ThenInclude(fr => fr.Inventory)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null) return NotFound();

            var model = new FridgeRequestViewModel
            {
                RequestId = request.RequestId,
                ContactNumber = request.ContactNumber,
                SpecialInstructions = request.SpecialInstructions,
                PreferredPickup = request.PreferredPickup,
                BusinessId = request.BusinessId,
                BusinessName = request.Customer.BusinessName,
                CustomerLocationId = request.CustomerLocationId,
                BranchName = request.CustomerLocation.BranchName,
                RequestedFridges = request.FridgeRequestItems.Select(f => new FridgeRequestItemViewModel
                {
                    InventoryId = f.Fridge.Inventory.InventoryId,
                    Model = f.Fridge.Inventory.Model,
                    Quantity = f.Quantity   
                }).ToList()
            };

            return View("ApproveRequest", model);
        }

        // POST: Approve Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequest(int id, string? notes)
        {
            var request = await _context.FridgeRequests
                .Include(r => r.Customer)
                .Include(r => r.CustomerLocation)
                .Include(r => r.FridgeRequestItems)
                    .ThenInclude(f => f.Fridge)
                        .ThenInclude(fr => fr.Inventory)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null) return NotFound();

            // Get logged-in liaison
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.ApplicationUserId == userId);

            if (employee == null)
            {
                TempData["Error"] = "You are not authorized to approve requests.";
                return RedirectToAction(nameof(FridgeRequests));
            }

            // Mark request as reviewed
            request.ReviewedById = employee.EmployeeId;
            request.ApprovedDate = DateTime.Now;
            request.SpecialInstructions = notes; // optional notes
            _context.Update(request);

            // Notify customer of approval
            var notification = new CustomerNotification
            {
                BusinessId = request.BusinessId,
                Message = $"Your fridge request at {request.CustomerLocation.BranchName} was approved. Please wait for allocation.",
                SentDate = DateTime.Now,
                IsRead = false
            };
            _context.CustomersNotifications.Add(notification);

            await _context.SaveChangesAsync();

            // Pass preselected data to CreateAllocation via TempData
            TempData["PreselectCustomerId"] = request.BusinessId;
            TempData["PreselectLocationId"] = request.CustomerLocationId;
            TempData["RequestedFridges"] = Newtonsoft.Json.JsonConvert.SerializeObject(
                request.FridgeRequestItems.Select(f => new { f.Fridge.Inventory.InventoryId, f.Quantity })
            );

            TempData["Success"] = "Fridge request approved. Proceed to allocation.";
            return RedirectToAction("CreateAllocation", "FridgeAllocations");
        }

        // POST: Reject Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int id, string reason)
        {
            var request = await _context.FridgeRequests
                .Include(r => r.Customer)
                .Include(r => r.CustomerLocation)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null) return NotFound();

            request.ReviewedById = _context.Employees
                .FirstOrDefault(e => e.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))?.EmployeeId;
            request.SpecialInstructions = reason; // store rejection reason
            _context.Update(request);

            // Notify the customer
            var notification = new CustomerNotification
            {
                BusinessId = request.BusinessId,
                Message = $"Your fridge request at {request.CustomerLocation.BranchName} was rejected. Reason: {reason}",
                SentDate = DateTime.Now,
                IsRead = false
            };
            _context.CustomersNotifications.Add(notification);

            await _context.SaveChangesAsync();

            TempData["Info"] = "Fridge request rejected.";
            return RedirectToAction(nameof(FridgeRequests));
        }

        // GET: Get Request Details (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetRequestDetails(int id)
        {
            var request = await _context.FridgeRequests
                .Include(r => r.Customer)
                .Include(r => r.CustomerLocation)
                .Include(r => r.FridgeRequestItems)
                    .ThenInclude(f => f.Fridge)
                        .ThenInclude(fr => fr.Inventory)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null) return NotFound();

            return Json(new
            {
                request.RequestId,
                request.ContactNumber,
                request.SpecialInstructions,
                request.PreferredPickup,
                BusinessName = request.Customer.BusinessName,
                BranchName = request.CustomerLocation.BranchName,
                Fridges = request.FridgeRequestItems.Select(f => new { f.Fridge.Inventory.Model, f.Quantity })
            });
        }
    }
}
