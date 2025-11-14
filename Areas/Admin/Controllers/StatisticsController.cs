using FridgeSystem.Data;
using FridgeSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")] //allows only the admin to do this
    [Area("Admin")]
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserController> _logger;

        public StatsController(
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

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            // Default period: last 6 months
            startDate ??= DateTime.Now.AddMonths(-6);
            endDate ??= DateTime.Now;

            // Rentals Trend (Fridges Picked Up by Month)
            var rentals = _context.CustomerFridges
                .Where(cf => cf.DatePickedUp >= startDate && cf.DatePickedUp <= endDate)
                .GroupBy(cf => new { cf.DatePickedUp.Year, cf.DatePickedUp.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .AsEnumerable()  // switch to client-side
                .Select(x => new
                {
                    Month = $"{x.Month}/{x.Year}",
                    x.Count
                })
                .ToList();

            // Fault Trends (Faults Logged by Month)
            var faultsQuery = await _context.FridgeFaults
                .Where(f => f.LoggedDate >= startDate && f.LoggedDate <= endDate)
                .GroupBy(f => new { f.LoggedDate.Year, f.LoggedDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var faults = faultsQuery
                .Select(x => new
                {
                    Month = $"{x.Month}/{x.Year}",
                    x.Count,
                    FaultToRentalRatio = Math.Round((double)x.Count / (rentals.Sum(r => r.Count) + 1) * 100, 2)
                })
                .ToList();

            // Maintenance Success Rate (from ServiceHistory)
            var maintenanceRecords = await _context.ServiceHistory
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .ToListAsync();

            double totalServices = maintenanceRecords.Count;
            double successful = maintenanceRecords.Count(s =>
                s.Notes != null && (s.Notes.Contains("Completed") || s.Notes.Contains("Success")));

            double maintenanceSuccessRate = totalServices == 0 ? 0 :
                Math.Round(successful / totalServices * 100, 2);

            double maintenanceFailedRate = totalServices == 0 ? 0 :
                Math.Round((totalServices - successful) / totalServices * 100, 2);

            // Business Acquisition Rate (New Customers)
            var businessDataRaw = await _context.Customers
                .Where(b => b.DateCreated >= startDate && b.DateCreated <= endDate && !b.isDeleted)
                .GroupBy(b => new { b.DateCreated.Year, b.DateCreated.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            var businessData = businessDataRaw
                .Select(x => new
                {
                    Month = $"{x.Month}/{x.Year}",
                    x.Count
                })
                .ToList();

            // Average monthly growth
            double avgMonthlyGrowth = businessData.Count > 1
                ? Math.Round(
                    ((double)businessData.Last().Count - (double)businessData.First().Count)
                    / Math.Max(1, businessData.First().Count) * 100,
                    2
                  )
                : 0;

            // Top 5 Businesses by Number of Rentals
            var topBusinesses = await _context.CustomerFridges
                .Include(cf => cf.CustomerLocation)
                .ThenInclude(cl => cl.Customer)
                .Where(cf => cf.CustomerLocation.Customer != null)
                .GroupBy(cf => cf.CustomerLocation.Customer.BusinessName)
                .Select(g => new
                {
                    BusinessName = g.Key,
                    RentalCount = g.Count()
                })
                .OrderByDescending(g => g.RentalCount)
                .Take(5)
                .ToListAsync();

            // Most Common Fault Types
            var faultTypes = await _context.FridgeFaults
                .Where(f => f.LoggedDate >= startDate && f.LoggedDate <= endDate)
                .GroupBy(f => f.FaultDescription)
                .Select(g => new
                {
                    FaultType = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToListAsync();

            // Prepare data for the View
            ViewBag.RentalsTrend = rentals;
            ViewBag.FaultTrends = faults;
            ViewBag.MaintenanceSuccessRate = maintenanceSuccessRate;
            ViewBag.MaintenanceFailedRate = maintenanceFailedRate; // <-- added
            ViewBag.CustomerGrowthRate = avgMonthlyGrowth;
            ViewBag.CustomerTrends = businessData;
            ViewBag.TopCustomers = topBusinesses;
            ViewBag.CommonFaults = faultTypes;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View();
        }
    }
}


