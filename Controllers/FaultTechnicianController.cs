using FridgeSystem.Data;
using FridgeSystem.Models;
using FridgeSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FridgeSystem.Controllers
{
    
    public class FaultTechnicianController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FaultTechnicianController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var technician = await _context.Employees
                .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

            if (technician == null)
                return Unauthorized("Technician profile not found.");

            // Fetch all assigned faults for this technician
            var faults = await _context.AssignedFaults
                .Include(a => a.FridgeFault)
                    .ThenInclude(f => f.CustomerFridge)
                        .ThenInclude(cf => cf.CustomerLocation)
                            .ThenInclude(cl => cl.Customer)
                .Where(a => a.TechnicianId == technician.EmployeeId && a.FridgeFault != null)
                .Select(a => a.FridgeFault)
                .ToListAsync();

            // Prepare dashboard view model
            var viewModel = new TechnicianDashboardViewModel
            {
                TechnicianName = $"{technician.FirstName} {technician.LastName}",
                NewlyAssigned = faults.Where(f => f.Status == FaultStatus.Assigned).ToList(),
                PendingDiagnosis = faults.Where(f => f.Status == FaultStatus.UnderDiagnosis).ToList(),
                ScheduledRepairs = faults.Where(f => f.Status == FaultStatus.Scheduled).ToList(),
                CompletedFaults = faults.Where(f => f.Status == FaultStatus.Completed).ToList()
            };

            // Prepare calendar events (for collapsible calendar)
            var schedules = await _context.ServiceSchedules
                .Include(s => s.FridgeFault)
                    .ThenInclude(f => f.CustomerFridge)
                        .ThenInclude(cf => cf.CustomerLocation)
                            .ThenInclude(cl => cl.Customer)
                .Where(s => s.EmployeeId == technician.EmployeeId)
                .Select(s => new
                {
                    id = s.ScheduleId,
                    title = "Repair: " + s.FridgeFault.FaultDescription,
                    start = s.StartDate,
                    end = s.EndDate ?? s.StartDate.AddHours(2),
                    customer = s.FridgeFault.CustomerFridge.CustomerLocation.Customer.BusinessName,
                    address = s.FridgeFault.CustomerFridge.CustomerLocation.BranchName,
                    status = s.Status.ToString() // Scheduled, InProgress, Completed
                })
                .ToListAsync();

            // Pass schedules to the view
            ViewData["Schedules"] = JsonConvert.SerializeObject(schedules);

            return View(viewModel);
        }



        public async Task<IActionResult> NewlyAssigned()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            // Find the technician profile
            var technician = await _context.Employees
                .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

            if (technician == null)
                return Unauthorized("Technician profile not found.");

            // Get faults assigned to this technician with Status = Assigned
            var faults = await _context.AssignedFaults
                .Include(a => a.FridgeFault)
                    .ThenInclude(f => f.CustomerFridge)
                        .ThenInclude(cf => cf.Fridge)
                .Include(a => a.FridgeFault)
                    .ThenInclude(f => f.CustomerFridge)
                        .ThenInclude(cf => cf.CustomerLocation)
                .Where(a => a.TechnicianId == technician.EmployeeId
                            && a.FridgeFault.Status == FaultStatus.Assigned)
                .Select(a => a.FridgeFault)
                .ToListAsync();

            return View(faults); // pass IEnumerable<FridgeFault> to the view
        }




        public async Task<IActionResult> FaultDetails(int id)
        {
            var fault = await _context.FridgeFaults
                .Include(f => f.CustomerFridge)
                    .ThenInclude(cf => cf.Fridge)
                .Include(f => f.CustomerFridge)
                    .ThenInclude(cf => cf.CustomerLocation)
                        .ThenInclude(cl => cl.Customer)
                .Include(f => f.AssignedFaults) // to know if assigned
                .FirstOrDefaultAsync(f => f.FridgeFaultId == id);

            if (fault == null)
                return NotFound();

            return View(fault);
        }





        public async Task<IActionResult> StartDiagnosis(int id)
        {
            var fault = await _context.FridgeFaults
        .Include(f => f.CustomerFridge)
            .ThenInclude(cf => cf.CustomerLocation)
                .ThenInclude(cl => cl.Customer)
        .Include(f => f.AssignedFaults)

        .FirstOrDefaultAsync(f => f.FridgeFaultId == id);

            if (fault == null) return NotFound();

            return View(fault);


        }

        //View Service Schedule (GET)
        public async Task<IActionResult> ServiceSchedule(int id)
        {
            var fault = await _context.FridgeFaults
        .Include(f => f.CustomerFridge)
            .ThenInclude(cf => cf.CustomerLocation)
            .ThenInclude(cl => cl.Customer)
        .Include(f => f.ServiceSchedules)
            .ThenInclude(ss => ss.Employee)
        .FirstOrDefaultAsync(f => f.FridgeFaultId == id);

            if (fault == null)
                return NotFound();

            // Show the same view, even if schedules are empty
            return View(fault);
        }

        public async Task<IActionResult> ScheduledRepairs()
        {
            var user = await _userManager.GetUserAsync(User);

            var technician = await _context.Employees
                .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

            if (technician == null)
                return Unauthorized("Technician profile not found.");

            var faults = await _context.FridgeFaults
                .Include(f => f.CustomerFridge)
                    .ThenInclude(cf => cf.CustomerLocation)
                    .ThenInclude(cl => cl.Customer)
                .Where(f => f.Status == FaultStatus.Scheduled
                         && f.ServiceSchedules.Any(ss => ss.EmployeeId == technician.EmployeeId))
                .ToListAsync();

            return View(faults);  // ← Create ScheduledRepairs.cshtml
        }

        public async Task<IActionResult> ScheduleCalendar()
        {
            var userId = _userManager.GetUserId(User); // string ID

            var schedules = await _context.ServiceSchedules
                .Include(s => s.FridgeFault)
                    .ThenInclude(f => f.CustomerFridge)
                        .ThenInclude(cf => cf.CustomerLocation)
                            .ThenInclude(cl => cl.Customer)
                .Include(s => s.Employee)
                .Where(s => s.Employee.ApplicationUserId == userId) // use string match
                .Select(s => new
                {
                    id = s.ScheduleId,
                    title = "Repair: " + s.FridgeFault.FaultDescription,
                    start = s.StartDate,
                    end = s.EndDate ?? s.StartDate.AddHours(2),
                    customer = s.FridgeFault.CustomerFridge.CustomerLocation.Customer.BusinessName,
                    address = s.FridgeFault.CustomerFridge.CustomerLocation.BranchName,
                    status = s.Status.ToString() // Scheduled, InProgress, Completed
                })
                .ToListAsync();

            ViewData["Schedules"] = JsonConvert.SerializeObject(schedules);

            return View();

        }





        // GET: FridgeFault/CreateServiceSchedule/5
        public async Task<IActionResult> CreateServiceSchedule(int FridgeFaultId)
        {
            var fault = await _context.FridgeFaults
                .Include(f => f.CustomerFridge)
                    .ThenInclude(cf => cf.CustomerLocation)
                        .ThenInclude(cl => cl.Customer)
                .FirstOrDefaultAsync(f => f.FridgeFaultId == FridgeFaultId);

            if (fault == null)
                return NotFound();

            // You can pass only the faultId or use a ViewModel if needed
            ViewBag.FaultId = FridgeFaultId;
            ViewBag.Customer = fault.CustomerFridge.CustomerLocation.Customer.BusinessName;
            ViewBag.ContactPerson = fault.CustomerFridge.CustomerLocation.ContactPerson;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateServiceSchedule(int FridgeFaultId, DateTime ScheduledDate, TimeSpan ScheduledTime)
        {
            var user = await _userManager.GetUserAsync(User);
            var technician = await _context.Employees
                .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

            if (technician == null)
                return Unauthorized("Technician profile not found.");

            // Get the fault
            var fault = await _context.FridgeFaults
                .Include(f => f.CustomerFridge)
                    .ThenInclude(cf => cf.CustomerLocation)
                        .ThenInclude(cl => cl.Customer)
                .FirstOrDefaultAsync(f => f.FridgeFaultId == FridgeFaultId);

            if (fault == null)
                return NotFound();

            // Combine date + time
            var dateTime = ScheduledDate.Date + ScheduledTime;

            var schedule = new ServiceSchedule
            {
                FridgeFaultId = fault.FridgeFaultId,
                FridgeId = fault.CustomerFridge.FridgeId,
                LocationId = fault.CustomerFridge.CustomerLocationId,
                EmployeeId = technician.EmployeeId,
                StartDate = dateTime,
                Status = FaultStatus.Scheduled,
            };

            _context.ServiceSchedules.Add(schedule);


            // ✅ Update fault status to Scheduled
            fault.Status = FaultStatus.Scheduled;
            await _context.SaveChangesAsync();

            // ✅ Create customer notification
            var notification = new CustomerNotification
            {
                BusinessId = fault.CustomerFridge.CustomerLocation.BusinessId,
                ScheduleId = schedule.ScheduleId, // ✅ After SaveChanges
                Message = $"Repair scheduled on {ScheduledDate:yyyy-MM-dd} with technician {technician.FirstName} {technician.LastName}.",
                SentDate = DateTime.Now,
                IsRead = false
            };

            _context.CustomersNotifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction("ServiceSchedule", new { id = FridgeFaultId });
        }

        [HttpPost]
        public async Task<IActionResult> CompleteRepair(int ScheduleId, DateTime FinishDate, TimeSpan FinishTime)
        {
            var schedule = await _context.ServiceSchedules
                .Include(s => s.FridgeFault)
                .FirstOrDefaultAsync(s => s.ScheduleId == ScheduleId);

            if (schedule == null)
                return NotFound();

            // Combine date and time
            schedule.EndDate = FinishDate.Date + FinishTime;
            schedule.Status = FaultStatus.Completed; // mark schedule as completed

            // Update fault status to completed so technician can create report
            schedule.FridgeFault.Status = FaultStatus.Completed;

            await _context.SaveChangesAsync();

            // Redirect to fault report creation form
            return RedirectToAction("CreateFaultReport", new { faultId = schedule.FridgeFaultId });
        }


        public async Task<IActionResult> CreateFaultReport(int faultId)
        {
            var fault = await _context.FridgeFaults
                .Include(f => f.CustomerFridge)
                    .ThenInclude(cf => cf.CustomerLocation)
                        .ThenInclude(cl => cl.Customer)
                .FirstOrDefaultAsync(f => f.FridgeFaultId == faultId);

            if (fault == null)
                return NotFound();

            return View(fault);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFaultReport(int faultId, string ReportDescription)
        {
            var fault = await _context.FridgeFaults.FindAsync(faultId);
            if (fault == null) return NotFound();

            var report = new FaultReport
            {
                FridgeFaultId = faultId,
                Description = ReportDescription,
                ReportedDate = DateTime.Now
            };

            _context.FaultReports.Add(report);
            await _context.SaveChangesAsync();

            // Optionally update fault or notify admin
            return RedirectToAction("Index"); // back to dashboard
        }


    }
}
