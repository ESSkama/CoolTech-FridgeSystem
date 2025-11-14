using FridgeSystem.Data;
using FridgeSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using FridgeSystem.Areas.Admin;

namespace FridgeSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
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


        // Utilities

        private string GenerateTemporaryPassword()
        {
            // Simple temporary password used for display in TempData
            return "Tmp@" + Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        private async Task EnsureRoleExistsAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                _logger.LogInformation($"Role '{roleName}' created successfully.");
            }
        }


        // EMPLOYEES
        // GET: Admin/User/Employees
        public async Task<IActionResult> ManageEmployee(string? searchText, string? statusFilter, string? roleFilter, string? provinceFilter, string? cityFilter, string? warehouseFilter)
        {
            var query = _context.Employees
            .Include(e => e.Warehouse)
            .AsQueryable();


            // Status filter
            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = statusFilter.Equals("active", StringComparison.OrdinalIgnoreCase)
                    ? query.Where(e => e.IsActive)
                    : query.Where(e => !e.IsActive);
            }

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim().ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(searchText) ||
                    e.LastName.ToLower().Contains(searchText) ||
                    e.Email.ToLower().Contains(searchText) ||
                    e.PhoneNumber.ToLower().Contains(searchText)
                );
            }

            // Province filter
            if (!string.IsNullOrWhiteSpace(provinceFilter))
                query = query.Where(e => e.Province == provinceFilter);

            // City filter
            if (!string.IsNullOrWhiteSpace(cityFilter))
                query = query.Where(e => e.City == cityFilter);

            // Warehouse filter
            if (!string.IsNullOrWhiteSpace(warehouseFilter))
            {
                if (int.TryParse(warehouseFilter, out int wid))
                    query = query.Where(e => e.WarehouseId == wid);
            }

            var employees = await query.OrderBy(e => e.FirstName).ToListAsync();


            var predefinedRoles = new List<string>
            {
               "Admin", "Customer Liaison", "Inventory Liaison", "Fault Technician", "Maintenance Technician"
            };

            var allRolesSet = new HashSet<string>(predefinedRoles, StringComparer.OrdinalIgnoreCase);

            var provinces = new List<string>
            {
              "Gauteng","Mpumalanga","KwaZulu-Natal","North West","Limpopo",
              "Western Cape","Free State","Eastern Cape","Northern Cape"
            };


            // Cities for each province

            var provinceCities = new Dictionary<string, List<string>>
            {
               { "Gauteng", new List<string> { "Johannesburg", "Pretoria", "Vereeniging" } },
               { "Western Cape", new List<string> { "Cape Town", "George", "Stellenbosch" } },
               { "KwaZulu-Natal", new List<string> { "Durban", "Pietermaritzburg", "Richards Bay" } },
               { "Free State", new List<string> { "Bloemfontein", "Welkom", "Kroonstad" } },
               { "Limpopo", new List<string> { "Polokwane", "Tzaneen", "Thohoyandou" } },
               { "North West", new List<string> { "Rustenburg", "Mahikeng", "Klerksdorp" } },
               { "Eastern Cape", new List<string> { "Port Elizabeth", "East London", "Mthatha" } },
               { "Northern Cape", new List<string> { "Kimberley", "Upington", "Springbok" } },
               { "Mpumalanga", new List<string> { "Nelspruit", "Witbank", "Ermelo" } }
            };


            var model = new List<EmployeeViewModel>();
            foreach (var emp in employees)
            {
                var vm = new EmployeeViewModel
                {
                    EmployeeId = emp.EmployeeId,
                    FirstName = emp.FirstName,
                    LastName = emp.LastName,
                    Email = emp.Email,
                    Username = emp.Email,
                    PhoneNumber = emp.PhoneNumber,
                    HiredDate = emp.HiredDate,
                    IsActive = emp.IsActive,
                    ApplicationUserId = emp.ApplicationUserId,
                    City = emp.City,
                    Province = emp.Province,
                    WarehouseId = emp.WarehouseId,
                    Warehouse = emp.Warehouse?.Name
                };

                // Load roles for each employee
                if (!string.IsNullOrEmpty(emp.ApplicationUserId))
                {
                    var user = await _userManager.FindByIdAsync(emp.ApplicationUserId);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        vm.RoleNames = roles;
                        foreach (var r in roles) allRolesSet.Add(r);
                    }
                }

                model.Add(vm);
            }

            // Roles
            var roleOptions = allRolesSet.OrderBy(r => r)
                .Select(r => new SelectListItem
                {
                    Text = r,
                    Value = r,
                    Selected = r.Equals(roleFilter, StringComparison.OrdinalIgnoreCase)
                }).ToList();

            // Provinces
            var provinceOptions = provinces.OrderBy(p => p)
                .Select(p => new SelectListItem
                {
                    Text = p,
                    Value = p,
                    Selected = p.Equals(provinceFilter, StringComparison.OrdinalIgnoreCase)
                }).ToList();

            // Cities (filtered by selected province)
            var selectedProvince = provinceFilter ?? provinces.First();
            var cityOptions = provinceCities.ContainsKey(selectedProvince)
                ? provinceCities[selectedProvince]
                    .Select(c => new SelectListItem
                    {
                        Text = c,
                        Value = c,
                        Selected = c.Equals(cityFilter, StringComparison.OrdinalIgnoreCase)
                    })
                    .ToList()
                : new List<SelectListItem>();

            // Warehouse options for dropdown (all warehouses)
            var warehouseOptions = await _context.Warehouses
                .Select(w => new SelectListItem
                {
                    Value = w.WarehouseId.ToString(),
                    Text = w.Name,
                    Selected = w.WarehouseId.ToString() == warehouseFilter
                })
                .ToListAsync();

            // ROLE FILTER

            if (!string.IsNullOrWhiteSpace(roleFilter))
            {
                var usersInRole = (await _userManager.GetUsersInRoleAsync(roleFilter))
                    .Select(u => u.Id)
                    .ToList();

                model = model.Where(m => usersInRole.Contains(m.ApplicationUserId)).ToList();
            }

            ViewBag.RoleOptions = roleOptions;
            ViewBag.ProvinceOptions = provinceOptions;
            ViewBag.CityOptions = cityOptions;
            ViewBag.WarehouseOptions = warehouseOptions;

            ViewBag.SearchText = searchText;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.RoleFilter = roleFilter;
            ViewBag.ProvinceFilter = provinceFilter;
            ViewBag.CityFilter = cityFilter;
            ViewBag.WarehouseFilter = warehouseFilter;

            return View("ManageEmployee", model);
        }


        // GET: Admin/User/AddEmployee
        public IActionResult AddEmployee()
        {
            // Provinces
            var provinces = new List<string>
    {
        "Gauteng","Mpumalanga","KwaZulu-Natal","North West","Limpopo",
        "Western Cape","Free State","Eastern Cape","Northern Cape"
    };

            // Cities by province
            var provinceCities = new Dictionary<string, List<string>>
    {
        { "Gauteng", new List<string> { "Johannesburg", "Pretoria", "Vereeniging" } },
        { "Western Cape", new List<string> { "Cape Town", "George", "Stellenbosch" } },
        { "KwaZulu-Natal", new List<string> { "Durban", "Pietermaritzburg", "Richards Bay" } },
        { "Free State", new List<string> { "Bloemfontein", "Welkom", "Kroonstad" } },
        { "Limpopo", new List<string> { "Polokwane", "Tzaneen", "Thohoyandou" } },
        { "North West", new List<string> { "Rustenburg", "Mahikeng", "Klerksdorp" } },
        { "Eastern Cape", new List<string> { "Port Elizabeth", "East London", "Mthatha" } },
        { "Northern Cape", new List<string> { "Kimberley", "Upington", "Springbok" } },
        { "Mpumalanga", new List<string> { "Nelspruit", "Witbank", "Ermelo" } }
    };

            // Roles
            var roles = new List<string>
    {
        "Admin", "Customer Liaison", "Inventory Liaison", "Fault Technician", "Maintenance Technician"
    };

            // Default selected province & city
            var defaultProvince = "Gauteng";
            var defaultCity = provinceCities[defaultProvince].First();

            // Fetch all warehouses (show all, not filtered by city)
            var warehouses = _context.Warehouses
                .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                .ToList();

            var model = new EmployeeViewModel
            {
                Provinces = provinces.Select(p => new SelectListItem { Value = p, Text = p }).ToList(),
                Cities = provinceCities[defaultProvince].Select(c => new SelectListItem { Value = c, Text = c }).ToList(),
                Warehouses = warehouses,
                Roles = roles.Select(r => new SelectListItem { Value = r, Text = r }).ToList(),
                IsActive = true
            };

            ViewBag.ProvinceCities = provinceCities; // Needed for JS filtering
            return View("AddEmployee", model);
        }


        // POST: Admin/User/AddEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(EmployeeViewModel model)
        {
            // Provinces
            var provinces = new List<string>
    {
        "Gauteng","Mpumalanga","KwaZulu-Natal","North West","Limpopo",
        "Western Cape","Free State","Eastern Cape","Northern Cape"
    };

            // Cities by province
            var provinceCities = new Dictionary<string, List<string>>
    {
        { "Gauteng", new List<string> { "Johannesburg", "Pretoria", "Vereeniging" } },
        { "Western Cape", new List<string> { "Cape Town", "George", "Stellenbosch" } },
        { "KwaZulu-Natal", new List<string> { "Durban", "Pietermaritzburg", "Richards Bay" } },
        { "Free State", new List<string> { "Bloemfontein", "Welkom", "Kroonstad" } },
        { "Limpopo", new List<string> { "Polokwane", "Tzaneen", "Thohoyandou" } },
        { "North West", new List<string> { "Rustenburg", "Mahikeng", "Klerksdorp" } },
        { "Eastern Cape", new List<string> { "Port Elizabeth", "East London", "Mthatha" } },
        { "Northern Cape", new List<string> { "Kimberley", "Upington", "Springbok" } },
        { "Mpumalanga", new List<string> { "Nelspruit", "Witbank", "Ermelo" } }
    };

            // Roles
            var roles = new List<string>
    {
        "Admin", "Customer Liaison", "Inventory Liaison", "Fault Technician", "Maintenance Technician"
    };

            // Validation check
            if (!ModelState.IsValid)
            {
                model.Provinces = provinces.Select(p => new SelectListItem { Value = p, Text = p }).ToList();
                model.Cities = provinceCities.ContainsKey(model.Province)
                    ? provinceCities[model.Province].Select(c => new SelectListItem { Value = c, Text = c }).ToList()
                    : new List<SelectListItem>();

                // Always show all warehouses
                model.Warehouses = _context.Warehouses
                    .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                    .ToList();

                model.Roles = roles.Select(r => new SelectListItem { Value = r, Text = r }).ToList();

                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return View("AddEmployee", model);
            }

            // Check for duplicate email
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "A user with this email already exists.");
                model.Provinces = provinces.Select(p => new SelectListItem { Value = p, Text = p }).ToList();
                model.Cities = provinceCities.ContainsKey(model.Province)
                    ? provinceCities[model.Province].Select(c => new SelectListItem { Value = c, Text = c }).ToList()
                    : new List<SelectListItem>();

                // Always show all warehouses
                model.Warehouses = _context.Warehouses
                    .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                    .ToList();

                model.Roles = roles.Select(r => new SelectListItem { Value = r, Text = r }).ToList();
                return View("AddEmployee", model);
            }

            // Create ApplicationUser
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            string passwordToUse = string.IsNullOrWhiteSpace(model.Password)
                ? Guid.NewGuid().ToString("N").Substring(0, 8)
                : model.Password;

            var userResult = await _userManager.CreateAsync(user, passwordToUse);
            if (!userResult.Succeeded)
            {
                foreach (var error in userResult.Errors)
                    ModelState.AddModelError("", error.Description);

                model.Provinces = provinces.Select(p => new SelectListItem { Value = p, Text = p }).ToList();
                model.Cities = provinceCities.ContainsKey(model.Province)
                    ? provinceCities[model.Province].Select(c => new SelectListItem { Value = c, Text = c }).ToList()
                    : new List<SelectListItem>();

                // Always show all warehouses
                model.Warehouses = _context.Warehouses
                    .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                    .ToList();

                model.Roles = roles.Select(r => new SelectListItem { Value = r, Text = r }).ToList();

                return View("AddEmployee", model);
            }

            // Assign selected role
            if (!string.IsNullOrEmpty(model.SelectedRole))
                await _userManager.AddToRoleAsync(user, model.SelectedRole);

            // Create Employee
            var employee = new Employee
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                City = model.City,
                Province = model.Province,
                HiredDate = DateTime.UtcNow,
                IsActive = model.IsActive,
                ApplicationUserId = user.Id,
                WarehouseId = model.WarehouseId
            };

            
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            
            var notification = new EmployeeNotification
            {
                EmployeeId = employee.EmployeeId,
                Message = $"Your account has been created. Username: {employee.Email}, Password: {passwordToUse}",
                SentDate = DateTime.UtcNow
            };

            _context.EmployeesNotifications.Add(notification);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Employee added successfully. Password: {passwordToUse}";
            return RedirectToAction("ManageEmployee");
        }




        // GET: Admin/User/EditEmployee/5
        public async Task<IActionResult> EditEmployee(int id)
        {
            var emp = await _context.Employees
                .Include(e => e.Warehouse)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (emp == null) return NotFound();

            // Roles
            var roles = new List<string>
    {
        "Admin", "Customer Liaison", "Inventory Liaison", "Fault Technician", "Maintenance Technician"
    };

            // Provinces and cities
            var provinceCities = new Dictionary<string, List<string>>
    {
        { "Gauteng", new List<string> { "Johannesburg", "Pretoria", "Midrand", "Soweto" } },
        { "Mpumalanga", new List<string> { "Nelspruit", "Witbank", "Secunda" } },
        { "KwaZulu-Natal", new List<string> { "Durban", "Pietermaritzburg", "Richards Bay" } },
        { "North West", new List<string> { "Rustenburg", "Mafikeng", "Klerksdorp" } },
        { "Limpopo", new List<string> { "Polokwane", "Thohoyandou", "Tzaneen" } },
        { "Western Cape", new List<string> { "Cape Town", "Stellenbosch", "Paarl" } },
        { "Free State", new List<string> { "Bloemfontein", "Welkom", "Sasolburg" } },
        { "Eastern Cape", new List<string> { "Port Elizabeth", "East London", "Mthatha" } },
        { "Northern Cape", new List<string> { "Kimberley", "Upington", "Springbok" } }
    };

            // Get user's current role
            string selectedRole = "";
            if (!string.IsNullOrEmpty(emp.ApplicationUserId))
            {
                var user = await _userManager.FindByIdAsync(emp.ApplicationUserId);
                if (user != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    selectedRole = userRoles.FirstOrDefault() ?? "";
                }
            }

            // Cities for the employee's current province
            var cities = provinceCities.ContainsKey(emp.Province)
                ? provinceCities[emp.Province]
                : new List<string>();

            // Warehouses for dropdown (show all)
            var warehouses = await _context.Warehouses.ToListAsync();

            var model = new EmployeeViewModel
            {
                EmployeeId = emp.EmployeeId,
                ApplicationUserId = emp.ApplicationUserId,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Email = emp.Email,
                Username = emp.Email,
                PhoneNumber = emp.PhoneNumber,
                City = emp.City,
                Province = emp.Province,
                IsActive = emp.IsActive,
                SelectedRole = selectedRole,
                WarehouseId = emp.WarehouseId,

                Roles = roles.Select(r => new SelectListItem
                {
                    Text = r,
                    Value = r,
                    Selected = r == selectedRole
                }).ToList(),

                Provinces = provinceCities.Keys.Select(p => new SelectListItem
                {
                    Text = p,
                    Value = p,
                    Selected = p == emp.Province
                }).ToList(),

                Cities = cities.Select(c => new SelectListItem
                {
                    Text = c,
                    Value = c,
                    Selected = c == emp.City
                }).ToList(),

                Warehouses = warehouses.Select(w => new SelectListItem
                {
                    Text = w.Name,
                    Value = w.WarehouseId.ToString(),
                    Selected = w.WarehouseId == emp.WarehouseId
                }).ToList()
            };

            ViewBag.ProvinceCities = provinceCities;
            return View("EditEmployee", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployee(EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns
                var roles = new List<string>
        {
            "Admin", "Customer Liaison", "Inventory Liaison", "Fault Technician", "Maintenance Technician"
        };

                var provinceCities = new Dictionary<string, List<string>>
        {
            { "Gauteng", new List<string> { "Johannesburg", "Pretoria", "Midrand", "Soweto" } },
            { "Mpumalanga", new List<string> { "Nelspruit", "Witbank", "Secunda" } },
            { "KwaZulu-Natal", new List<string> { "Durban", "Pietermaritzburg", "Richards Bay" } },
            { "North West", new List<string> { "Rustenburg", "Mafikeng", "Klerksdorp" } },
            { "Limpopo", new List<string> { "Polokwane", "Thohoyandou", "Tzaneen" } },
            { "Western Cape", new List<string> { "Cape Town", "Stellenbosch", "Paarl" } },
            { "Free State", new List<string> { "Bloemfontein", "Welkom", "Sasolburg" } },
            { "Eastern Cape", new List<string> { "Port Elizabeth", "East London", "Mthatha" } },
            { "Northern Cape", new List<string> { "Kimberley", "Upington", "Springbok" } }
        };

                var cities = provinceCities.ContainsKey(model.Province)
                    ? provinceCities[model.Province]
                    : new List<string>();

                // Always show all warehouses
                var warehouses = await _context.Warehouses.ToListAsync();

                model.Roles = roles.Select(r => new SelectListItem
                {
                    Text = r,
                    Value = r,
                    Selected = r == model.SelectedRole
                }).ToList();

                model.Provinces = provinceCities.Keys.Select(p => new SelectListItem
                {
                    Text = p,
                    Value = p,
                    Selected = p == model.Province
                }).ToList();

                model.Cities = cities.Select(c => new SelectListItem
                {
                    Text = c,
                    Value = c,
                    Selected = c == model.City
                }).ToList();

                model.Warehouses = warehouses.Select(w => new SelectListItem
                {
                    Text = w.Name,
                    Value = w.WarehouseId.ToString(),
                    Selected = w.WarehouseId == model.WarehouseId
                }).ToList();

                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return View("EditEmployee", model);
            }

            var employee = await _context.Employees.FindAsync(model.EmployeeId);
            if (employee == null) return NotFound();

            // Update employee info
            employee.FirstName = model.FirstName;
            employee.LastName = model.LastName;
            employee.Email = model.Email;
            employee.PhoneNumber = model.PhoneNumber;
            employee.City = model.City;
            employee.Province = model.Province;
            employee.WarehouseId = model.WarehouseId;
            employee.IsActive = model.IsActive;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            // Update linked ApplicationUser
            if (!string.IsNullOrEmpty(employee.ApplicationUserId))
            {
                var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    await _userManager.UpdateAsync(user);

                    if (!string.IsNullOrWhiteSpace(model.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, model.Password);
                    }

                    var currentRoles = await _userManager.GetRolesAsync(user);
                    if (!string.IsNullOrEmpty(model.SelectedRole) && !currentRoles.Contains(model.SelectedRole))
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    }
                }
            }

            TempData["SuccessMessage"] = "Employee details updated successfully.";
            return RedirectToAction("ManageEmployee");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee not found.";
                return RedirectToAction("ManageEmployee");
            }

            // Soft delete: mark as inactive
            employee.IsActive = false;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Employee has been deactivated.";
            return RedirectToAction("ManageEmployee");
        }



        // GET: Admin/User/CreateCustomer
        [HttpGet]
        public async Task<IActionResult> CreateCustomer()
        {
            var model = new CustomerViewModel
            {
                ProfileActive = true,
                BusinessTypeList = _context.BusinessTypes
            .Select(bt => new SelectListItem
            {
                Value = bt.BusinessTypeId.ToString(),
                Text = bt.TypeName
            }).ToList()
            };



            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer(CustomerViewModel model)
        {
            // --- Existing Password Validation (if applicable) ---
            // If you intend for a password to be REQUIRED for new customers:
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", "Password is required for a new customer.");
            }
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");
            }

            if (!ModelState.IsValid)
            {
                // Repopulate dropdown if validation fails
                model.BusinessTypeList = await _context.BusinessTypes // Use await and ToListAsync()
                    .Select(bt => new SelectListItem
                    {
                        Value = bt.BusinessTypeId.ToString(),
                        Text = bt.TypeName
                    }).ToListAsync();
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View(model);
            }

            // --- 1. Create the NEW ApplicationUser account FIRST ---
            var newApplicationUser = new ApplicationUser
            {
                UserName = model.Email, // Often email is used as username
                Email = model.Email,
                EmailConfirmed = true, // Or false if you want them to verify email
                                       // Add any other properties for your ApplicationUser here, e.g.,
                                       // FirstName = model.ContactPersonName, (if your ApplicationUser has it)
                                       // LastName = "...",
            };

            // Use UserManager to create the new user with their password
            var createResult = await _userManager.CreateAsync(newApplicationUser, model.Password);

            if (!createResult.Succeeded)
            {
                // If user creation failed, add errors to ModelState and return to view
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                model.BusinessTypeList = await _context.BusinessTypes // Repopulate dropdown
                    .Select(bt => new SelectListItem
                    {
                        Value = bt.BusinessTypeId.ToString(),
                        Text = bt.TypeName
                    }).ToListAsync();
                TempData["ErrorMessage"] = "Failed to create customer login account.";
                return View(model);
            }


            // ✅ Assign "Customer" role to this new ApplicationUser
            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }
            await _userManager.AddToRoleAsync(newApplicationUser, "Customer");


            // --- 2. Now that the ApplicationUser is created and has an ID, create the Customer ---
            var customer = new Customer
            {
                BusinessName = model.BusinessName,
                Email = model.Email,
                Telephone = model.Telephone,
                BillingAddress = model.BillingAddress,
                BusinessTypeId = model.BusinessTypeId,
                ContactPersonName = model.ContactPersonName,
                ProfileActive = model.ProfileActive,
                DateCreated = DateTime.UtcNow,
                // --- THIS IS THE CRUCIAL CHANGE ---
                // Link the customer to the NEWLY CREATED ApplicationUser's ID
                ApplicationUserId = newApplicationUser.Id
            };

            _context.Customers.Add(customer);

            try
            {

                await _context.SaveChangesAsync(); // Save the customer
                TempData["SuccessMessage"] = "Customer and login account created successfully!";
                return RedirectToAction("ManageCustomer");
            }
            catch (DbUpdateException ex)
            {
                // If an error occurs saving the customer AFTER the user was created,
                // you might want to consider deleting the newly created ApplicationUser
                // to prevent orphaned user accounts. This requires more complex transaction handling.
                // For now, log the error and display a generic message.
                // Log the exception for debugging: _logger.LogError(ex, "Error saving customer after user creation.");

                // Optionally, try to delete the newly created user if customer save fails
                await _userManager.DeleteAsync(newApplicationUser); // This is a simple rollback attempt

                ModelState.AddModelError("", "An error occurred while saving the customer details. Please try again.");
                model.BusinessTypeList = await _context.BusinessTypes // Repopulate dropdown
                    .Select(bt => new SelectListItem
                    {
                        Value = bt.BusinessTypeId.ToString(),
                        Text = bt.TypeName
                    }).ToListAsync();
                TempData["ErrorMessage"] = "Failed to create customer details.";
                return View(model);
            }
        }









        [HttpGet]
        public async Task<IActionResult> ManageCustomer(string? searchText, string? statusFilter)
        {
            var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);

            // Auto-deactivate inactive customers
            // Find customers where:
            // 1. Their ProfileActive is true
            // 2. AND (their LastLoginDate is null OR it's older than three months ago)
            // 3. AND they have no FridgeAllocations linked to ANY of their locations within the last three months
            // 4. AND they have no ServiceSchedules linked to ANY of their locations within the last three months
            var inactiveCustomers = await _context.Customers
                .Include(c => c.CustomerLocations) // Include locations to check for allocations/schedules
                .Include(c => c.BusinessType)
                .Where(c => c.ProfileActive &&
                    ((!c.LastLoginDate.HasValue || c.LastLoginDate < threeMonthsAgo) &&
                     !c.CustomerLocations.Any(cl => cl.FridgeAllocations.Any(fa => fa.AllocatedDate >= threeMonthsAgo)) &&
                     !c.CustomerLocations.Any(cl => cl.ServiceSchedules.Any(ss => ss.StartDate >= threeMonthsAgo))))
                .ToListAsync();

            foreach (var cust in inactiveCustomers)
            {
                cust.ProfileActive = false;
                _context.Update(cust);
            }

            if (inactiveCustomers.Any())
                await _context.SaveChangesAsync();

            // Base query
            var query = _context.Customers.Include(c => c.CustomerLocations).AsQueryable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim().ToLower();

                query = query.Where(c =>
                    c.BusinessName.ToLower().Contains(searchText) ||
                    c.Email.ToLower().Contains(searchText) ||
                    c.Telephone.ToLower().Contains(searchText) ||
                    c.ContactPersonName.ToLower().Contains(searchText) || // Added ContactPersonName
                    c.CustomerLocations.Any(l =>
                        l.BranchName.ToLower().Contains(searchText) ||
                        l.AddressLine1.ToLower().Contains(searchText) || // Added AddressLine1
                        l.AddressLine2.ToLower().Contains(searchText) || // Added AddressLine2
                        l.City.ToLower().Contains(searchText) ||
                        l.Province.ToLower().Contains(searchText) ||
                        l.Postcode.ToLower().Contains(searchText) ||     // Added Postcode
                        l.Telephone.ToLower().Contains(searchText) ||    // Added Location Telephone
                        l.ContactPerson.ToLower().Contains(searchText))); // Added Location ContactPerson
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                if (statusFilter.Equals("active", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(c => c.ProfileActive);
                else if (statusFilter.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(c => !c.ProfileActive);
            }

            // Execute and map
            var customers = await _context.Customers
                               .Include(c => c.BusinessType)
                                .Include(c => c.CustomerLocations) // <--- ADD THIS LINE to load CustomerLocations
                               .ToListAsync();

            var model = customers.Select(c => new CustomerViewModel // Assuming you still want CustomerViewModel
            {
                BusinessId = c.BusinessId,
                BusinessName = c.BusinessName,
                Email = c.Email,
                Telephone = c.Telephone,
                BillingAddress = c.BillingAddress,
                BusinessTypeId = c.BusinessTypeId,
                BusinessTypeName = c.BusinessType != null ? c.BusinessType.TypeName : "Unspecified",
                ContactPersonName = c.ContactPersonName,

                ProfileActive = c.ProfileActive,
                DateCreated = c.DateCreated,
                TotalLocations = c.CustomerLocations.Count
                // Add any other properties from CustomerViewModel if they exist and are applicable
                // For example, if CustomerViewModel had ContactPersonName, you'd add:
                // ContactPersonName = c.ContactPersonName,
            }).ToList();

            ViewBag.SearchText = searchText;
            ViewBag.StatusFilter = statusFilter;


            ViewBag.StatusOptions = new SelectList(
              new[]
              {
                    new { Value = "", Text = "All Statuses" },
                    new { Value = "active", Text = "Active" },
                    new { Value = "inactive", Text = "Inactive" }
              },
                "Value",
                "Text",
                statusFilter // This sets the selected value
            );


            return View("ManageCustomer", model);
        }

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
                Telephone = customer.Telephone,
                BillingAddress = customer.BillingAddress,
                BusinessTypeId = customer.BusinessTypeId,
                ContactPersonName = customer.ContactPersonName,
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

        // POST: Admin/User/EditCustomer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(int id, CustomerViewModel model)
        {
            if (id != model.BusinessId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                model.BusinessTypeList = await _context.BusinessTypes
                    .Select(bt => new SelectListItem
                    {
                        Value = bt.BusinessTypeId.ToString(),
                        Text = bt.TypeName
                    }).ToListAsync();

                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View(model);
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            customer.BusinessName = model.BusinessName;
            customer.Email = model.Email;
            customer.Telephone = model.Telephone;
            customer.BillingAddress = model.BillingAddress;
            customer.BusinessTypeId = model.BusinessTypeId;
            customer.ContactPersonName = model.ContactPersonName;
            customer.ProfileActive = model.ProfileActive;

            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Customer details updated successfully!";
                return RedirectToAction("ManageCustomer");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Customers.Any(e => e.BusinessId == model.BusinessId))
                    return NotFound();
                else
                    throw;
            }
        }

        [HttpGet]
        [Route("Admin/User/CustomerLocations/{BusinessId}")]
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



        // GET: Admin/FridgeRequest/Manage
        public async Task<IActionResult> ManageRequests()
        {
            var requests = await _context.FridgeRequests
                .Include(r => r.Customer) // Load customer info
                .Include(r => r.FridgeRequestItems)
                    .ThenInclude(i => i.Fridge) // Load fridge details
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.FridgeRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = RequestStatus.Approved;
            request.ApprovedDate = DateTime.UtcNow;

            _context.Update(request);

            var notification = new CustomerNotification
            {
                BusinessId = request.BusinessId,
                Message = $"Your fridge request #{request.RequestId} has been {request.Status}.",
                SentDate = DateTime.UtcNow
            };
            _context.CustomersNotifications.Add(notification);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Request #{id} approved.";
            return RedirectToAction(nameof(ManageRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(int id)
        {
            var request = await _context.FridgeRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = RequestStatus.Declined;
            request.ApprovedDate = DateTime.UtcNow; // optional, or create DeclinedDate

            _context.Update(request);
            var notification = new CustomerNotification
            {
                BusinessId = request.BusinessId,
                Message = $"Your fridge request #{request.RequestId} has been {request.Status}.",
                SentDate = DateTime.UtcNow
            };
            _context.CustomersNotifications.Add(notification);
            await _context.SaveChangesAsync();

            TempData["ErrorMessage"] = $"Request #{id} declined.";
            return RedirectToAction(nameof(ManageRequests));
        }


        public async Task<IActionResult> LoggedFaults()
        {
            var faults = await _context.FridgeFaults
            .Include(f => f.CustomerFridge)
                .ThenInclude(cf => cf.CustomerLocation)
                    .ThenInclude(cl => cl.Customer)
           .Include(f => f.AssignedFaults)                  // include assignments
            .ThenInclude(af => af.Technician)            // include technician (Employee)
        .Where(f => f.Status == FaultStatus.Logged || f.Status == FaultStatus.Assigned)
            .OrderByDescending(f => f.LoggedDate)
            .ToListAsync();

            return View(faults);
        }
        // GET: show assign form
        [HttpGet]
        public async Task<IActionResult> AssignFault(int faultId)
        {
            var warehouses = await _context.Warehouses
                .Select(w => new SelectListItem
                {
                    Value = w.WarehouseId.ToString(),
                    Text = w.Name
                })
                .ToListAsync();

            var model = new AssignFaultViewModel
            {
                FaultId = faultId,
                Warehouses = warehouses
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignFault(AssignFaultViewModel model)
        {
            // Rebuild warehouses dropdown
            model.Warehouses = await _context.Warehouses
                .Select(w => new SelectListItem
                {
                    Value = w.WarehouseId.ToString(),
                    Text = w.Name
                })
                .ToListAsync();

            // If warehouse selected but technician not yet, load technicians
            if (model.SelectedWarehouseId != 0 && model.SelectedTechnicianId == 0)
            {
                model.Technicians = await _context.Employees
                    .Where(t => t.WarehouseId == model.SelectedWarehouseId)
                    .Select(t => new TechnicianWithCount
                    {
                        TechnicianId = t.EmployeeId,
                        TechnicianName = t.FirstName + " " + t.LastName,
                        AssignedFaultCount = _context.AssignedFaults.Count(a => a.TechnicianId == t.EmployeeId)
                    })
                    .OrderBy(t => t.AssignedFaultCount)
                    .ToListAsync();

                TempData["InfoMessage"] = "Select a technician to assign/reassign.";
                return View(model);
            }

            // Assign or reassign fault if both warehouse and technician selected
            if (model.SelectedWarehouseId != 0 && model.SelectedTechnicianId != 0)
            {
                var fault = await _context.FridgeFaults
                    .Include(f => f.AssignedFaults)
                    .FirstOrDefaultAsync(f => f.FridgeFaultId == model.FaultId);

                if (fault == null) return NotFound();

                // Remove previous assignment if exists
                var existingAssignment = fault.AssignedFaults.FirstOrDefault();
                if (existingAssignment != null)
                {
                    _context.AssignedFaults.Remove(existingAssignment);
                }

                // Add new assignment
                var assignment = new AssignedFault
                {
                    FridgeFaultId = model.FaultId,
                    TechnicianId = model.SelectedTechnicianId,
                    AssignmentDate = DateTime.Now
                };

                fault.Status = FaultStatus.Assigned;

                _context.AssignedFaults.Add(assignment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = existingAssignment == null
                    ? "Technician successfully assigned!"
                    : "Technician reassigned successfully!";

                return RedirectToAction("LoggedFaults", "User");
            }

            TempData["ErrorMessage"] = "Please select both warehouse and technician.";
            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> ConfirmAssignment(int faultId, int technicianId)
        {
            var fault = await _context.FridgeFaults.FindAsync(faultId);
            if (fault == null) return NotFound();

            if (technicianId == 0)
            {
                TempData["ErrorMessage"] = "Please select a technician.";
                return RedirectToAction("AssignFault", new { faultId });
            }

            var assignment = new AssignedFault
            {
                FridgeFaultId = faultId,
                TechnicianId = technicianId,
                AssignmentDate = DateTime.Now
            };

            fault.Status = FaultStatus.Assigned;

            _context.AssignedFaults.Add(assignment);
            await _context.SaveChangesAsync();

            return RedirectToAction("LoggedFaults");
        }





        public async Task<IActionResult> FaultDetails(int id)
        {
            // Load all technicians (users who have the role "Fault Technician")
            var technicians = await (from user in _context.Users
                                     join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                     join role in _context.Roles on userRole.RoleId equals role.Id
                                     where role.Name == "Fault Technician"
                                     select user).ToListAsync();

            ViewBag.Technicians = technicians;

            // Load the fault details with full navigation properties
            var fault = await _context.FridgeFaults
                 .Include(f => f.CustomerFridge)
                     .ThenInclude(cf => cf.Fridge) // fridge details
                 .Include(f => f.CustomerFridge)
                     .ThenInclude(cf => cf.CustomerLocation) // branch
                         .ThenInclude(cl => cl.Customer)
                 .Include(f => f.CustomerFridge)
                     .ThenInclude(cf => cf.Warehouse) // warehouse for pickup
                 .FirstOrDefaultAsync(f => f.FridgeFaultId == id);

            if (fault == null) return NotFound();

            return View(fault);
        }




        // GET: CustomerFaults
        /*public async Task<IActionResult> MyFaults()
        {
            var userId = _userManager.GetUserId(User); // logged-in ApplicationUser
            var customer = await _context.Customers
                                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null)
                return NotFound("Customer profile not found.");

            // Get faults for this customer's fridges
            var faults = await _context.FridgeFaults
                                .Include(ff => ff.CustomerFridge)
                                .ThenInclude(cf => cf.Fridge)
                                .Where(ff => ff.CustomerFridge.CustomerLocation.Customer.BusinessId == customer.BusinessId)
                                .OrderByDescending(ff => ff.LoggedDate)
                                .ToListAsync();

            return View(faults);
        }*/

    }

}