using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YIL_CSD_Feedback_Management.Controllers
{
    [Authorize(Roles = "User,Manager")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.FullName = User.FindFirst("FullName")?.Value;

            ViewBag.Region = User.FindFirst("Region")?.Value;

            ViewBag.Module = User.FindFirst("Module")?.Value;

            ViewBag.DepartmentId = User.FindFirst("DepartmentId")?.Value;

            return View();
        }


    }
}