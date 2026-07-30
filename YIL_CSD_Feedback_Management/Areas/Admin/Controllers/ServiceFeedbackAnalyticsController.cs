using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceFeedbackAnalyticsController : Controller
    {
        private readonly IServiceFeedbackAnalyticsService _service;

        public ServiceFeedbackAnalyticsController(
            IServiceFeedbackAnalyticsService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(
            ServiceFeedbackAnalyticsViewModel model)
        {
            model = await _service.GetAnalyticsAsync(model);

            return View(model);
        }
        public async Task<IActionResult> RegionDetails(
    string region,
    int? month,
    int? year)
        {
            if (string.IsNullOrWhiteSpace(region))
            {
                return RedirectToAction(nameof(Index));
            }

            var data = await _service.GetRegionDetailsAsync(
                region,
                month,
                year);

            ViewBag.Region = region;
            ViewBag.Month = month;
            ViewBag.Year = year;

            return View(data);
        }

        public async Task<IActionResult> ExportToExcel(int? month, int? year)
        {
            return await _service.ExportToExcelAsync(month, year);
        }
    }
}