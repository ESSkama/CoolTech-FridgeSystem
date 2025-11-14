using FridgeSystem.Data;
using FridgeSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FridgeSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")] //allows only the admin to do this
    [Area("Admin")]
    public class FridgeRequestsController : Controller
    {

        private readonly ApplicationDbContext _context;
        public FridgeRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var requests = await _context.FridgeRequests
                .Include(r => r.Customer)
                .Include(r => r.FridgeRequestItems)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
            return View(requests);
        }

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
            var request = await _context.FridgeRequests
        .Include(r => r.FridgeRequestItems)
        .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null) return NotFound();

            request.Status = RequestStatus.Approved;
            request.ApprovedDate = DateTime.UtcNow;

            _context.Update(request);

            // Add CustomerFridges for each fridge in the request
            foreach (var item in request.FridgeRequestItems)
            {
                var customerFridge = new CustomerFridge
                {
                    CustomerId = request.BusinessId,
                    FridgeId = item.FridgeId,
                    CustomerLocationId = request.CustomerLocationId,
                    DatePickedUp = DateTime.UtcNow,
                    //SerialNumber = item.SerialNumber

                };
                _context.CustomerFridges.Add(customerFridge);
            }

            // Send notification
            var notification = new CustomerNotification
            {
                BusinessId = request.BusinessId,
                Message = $"Your fridge request #{request.RequestId} has been approved.",
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


        // GET: Admin/FridgeRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var request = await _context.FridgeRequests
                .Include(r => r.Customer) // Load customer info
                .Include(r => r.Warehouse) // Load warehouse info
                .Include(r => r.CustomerLocation) // Load branch/location
                .Include(r => r.FridgeRequestItems) // Load request items
                    .ThenInclude(i => i.Fridge) // Load fridge details
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null)
                return NotFound();

            return View(request);
        }



    }
}
