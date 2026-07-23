using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RegionDashboardController : Controller
    {
        private readonly IRegionDashboardService _service;

        public RegionDashboardController(
            IRegionDashboardService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _service.GetDashboardAsync();

            return View(model);
        }

        public async Task<IActionResult> List(string region)
        {
            if (string.IsNullOrWhiteSpace(region))
                return RedirectToAction(nameof(Index));

            var model = await _service.GetRegionCasesAsync(region);

            ViewBag.Region = region;

            return View(model);
        }
    }
}