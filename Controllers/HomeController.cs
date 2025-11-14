using FridgeSystem.Data;
using FridgeSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FridgeSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // -----------------------------------------------------
            // START: ROLE-BASED REDIRECT LOGIC
            // -----------------------------------------------------

            if (_signInManager.IsSignedIn(User))
            {
                // 1. Check for Customer Liaison
                if (User.IsInRole("Customer Liaison"))
                {
                    // Redirect to the CustomerLiaison Dashboard
                    return RedirectToAction("Dashboard", "CustomerLiaison");
                }

                // 2. Check for Inventory Liaison
                if (User.IsInRole("Inventory Liaison"))
                {
                    // Redirect to the InventoryLiaison Dashboard
                    return RedirectToAction("Dashboard", "InventoryLiaison");
                }
                if (User.IsInRole("Fault Technician"))
                {
                    // Redirect to the InventoryLiaison Dashboard
                    return RedirectToAction("Index", "FaultTechnician");
                }

                //check for fault teachnician


                /* // 3. Optional: Check for Admin to send them to their own landing page
                 if (User.IsInRole("Admin"))
                 {
                     // Assuming an Admin controller exists. Adjust as needed.
                     // If you want Admins to see the fridge catalog, comment this out.
                     // return RedirectToAction("Index", "Admin"); 
                 }*/
            }

            // -----------------------------------------------------
            // END: ROLE-BASED REDIRECT LOGIC
            // -----------------------------------------------------

            // Existing logic for guests, standard users, or unhandled roles (shows the fridge catalog)
            var fridges = await _context.Fridges
                .Include(f => f.Supplier)
                .Where(f => f.isDeleted == false || f.isDeleted == null)
                .ToListAsync();

            ViewBag.IsAdmin = _signInManager.IsSignedIn(User) && User.IsInRole("Admin");
            return View(fridges);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var fridge = await _context.Fridges
                // If you want to show Supplier/Brand Name
                .Include(f => f.Supplier)
                .FirstOrDefaultAsync(m => m.FridgeId == id && (m.isDeleted == false || m.isDeleted == null)); // Also filter deleted fridges her

            if (fridge == null)
            {
                return NotFound();
            }

            ViewBag.IsAdmin = _signInManager.IsSignedIn(User) && User.IsInRole("Admin"); // <--- KEY CHANGE HERE


            return View(fridge);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
