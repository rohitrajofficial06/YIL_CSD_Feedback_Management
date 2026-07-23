using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EngineerPerformanceController : Controller
    {
        private readonly IEngineerPerformanceService _service;

        public EngineerPerformanceController(
            IEngineerPerformanceService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _service.GetDashboardAsync();

            return View(model);
        }

        public async Task<IActionResult> Details(
     EngineerDetailsViewModel filter)
        {
            if (string.IsNullOrWhiteSpace(filter.EngineerName))
            {
                return RedirectToAction(nameof(Index));
            }

            var model =
                await _service.GetEngineerDetailsAsync(filter);

            return View(model);
        }
    }
}