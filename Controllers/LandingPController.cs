using System.Diagnostics;
using FridgeSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace FridgeSystem.Controllers
{
    public class LandingPController : Controller
    {
        private readonly ILogger<LandingPController> _logger;

        public LandingPController(ILogger<LandingPController> logger)
        {
            _logger = logger;
        }


        public IActionResult HomeLanding()
        {
            return View();
        }



        public IActionResult Index()
        {
            return View();
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