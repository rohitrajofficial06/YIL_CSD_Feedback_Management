using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceFeedbackAnalyticsController : Controller
    {
        private readonly IServiceFeedbackAnalyticsService _service;
        private readonly ILogService _logService;

        public ServiceFeedbackAnalyticsController(
     IServiceFeedbackAnalyticsService service,
     ILogService logService)
        {
            _service = service;
            _logService = logService;
        }

        public async Task<IActionResult> Index(
            ServiceFeedbackAnalyticsViewModel model)
        {
            model = await _service.GetAnalyticsAsync(model);

            await _logService.InformationAsync(
    "Service Feedback Analytics",
    "Index",
    $"Viewed Analytics Dashboard. Month : {model.Month}, Year : {model.Year}");

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

            await _logService.InformationAsync(
    "Service Feedback Analytics",
    "RegionDetails",
    $"Viewed Region Analytics. Region : {region}, Month : {month}, Year : {year}");

            ViewBag.Region = region;
            ViewBag.Month = month;
            ViewBag.Year = year;

            return View(data);
        }

        public async Task<IActionResult> ExportToExcel(int? month, int? year)
        {
            await _logService.InformationAsync(
                "Service Feedback Analytics",
                "ExportToExcel",
                $"Exported Analytics Report. Month : {month}, Year : {year}");

            return await _service.ExportToExcelAsync(month, year);
        }
    }
}