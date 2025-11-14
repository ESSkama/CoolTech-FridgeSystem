using FridgeSystem.Data;
using FridgeSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FridgeSystem.Controllers
{
    public class FaultController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FaultController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Create(int fridgeId)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            var fridge = await _context.CustomerFridges
                .Include(f => f.Fridge)
                .FirstOrDefaultAsync(f => f.CustomerFridgeId == fridgeId);

            if (fridge == null)
                return NotFound();

            var model = new FaultReportViewModel
            {
                BusinessId = customer.BusinessId,
                FridgeId = fridgeId,
                CategoryOptions = Enum.GetValues(typeof(FaultCategory))
                    .Cast<FaultCategory>()
                    .Select(c => new SelectListItem
                    {
                        Value = c.ToString(),
                        Text = c.ToString()
                    }).ToList()
            };

            return View(model);
        }


        // POST: Fault/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FaultReportViewModel model)
        {


            if (!ModelState.IsValid)
            {
                // Log or display all validation errors
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        // You can log this to console for debugging
                        Console.WriteLine($"Property: {key}, Error: {error.ErrorMessage}");
                    }
                }

                // Optionally, send the errors to TempData to display in the view
                TempData["ValidationErrors"] = string.Join("<br/>",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }
            if (!ModelState.IsValid)
            {
                // Re-populate dropdown in case of validation error
                model.CategoryOptions = Enum.GetValues(typeof(FaultCategory))
                    .Cast<FaultCategory>()
                    .Select(c => new SelectListItem
                    {
                        Value = c.ToString(),
                        Text = c.ToString()
                    }).ToList();

                return View(model);
            }

            var fault = new FridgeFault
            {
                CustomerFridgeId = model.FridgeId,
               SelectedCategory = model.SelectedCategory, // store the selected category
                FaultDescription = model.Description,
                Title = model.SelectedCategory.ToString(),
                LoggedDate = DateTime.Now,
                Status = FaultStatus.Logged,
            };

            _context.FridgeFaults.Add(fault);
            await _context.SaveChangesAsync();

            // Redirect to the FaultDetails page for this newly created fault
            return RedirectToAction("FaultDetails", new { id = fault.FridgeFaultId });
        }

        // GET: CustomerFaults
        public async Task<IActionResult> MyFaults()
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
        }
        // GET: Fault/Details/5
        public async Task<IActionResult> FaultDetails(int? id)
        {
            var fault = await _context.FridgeFaults
        .Include(f => f.CustomerFridge)
            .ThenInclude(cf => cf.Fridge)
        .Include(f => f.CustomerFridge)
            .ThenInclude(cf => cf.CustomerLocation)
                .ThenInclude(cl => cl.Customer)
        .Include(f => f.AssignedFaults)
            .ThenInclude(af => af.Technician)
        .FirstOrDefaultAsync(f => f.FridgeFaultId == id);

            if (fault == null)
                return NotFound();

            return View(fault);
        }

    }
}
