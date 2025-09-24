using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVServiceCenterMaintenanceSystem.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            ViewBag.Username = User.Identity?.Name ?? "Guest";
            return View();
        }

        public IActionResult Public()
        {
            return View();
        }
    }
}
