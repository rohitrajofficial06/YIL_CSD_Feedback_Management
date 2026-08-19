using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportsController : Controller
    {
        private readonly IReportsService _service;
        private readonly ILogService _logService;

        public ReportsController(
            IReportsService service,
            ILogService logService)
        {
            _service = service;
            _logService = logService;
        }

        // ==========================================================
        // Reports Dashboard
        // ==========================================================

        public async Task<IActionResult> Index()
        {
            var model = await _service.GetDashboardAsync();

            await _logService.InformationAsync(
                "Reports",
                "Index",
                "Viewed Reports Dashboard.");

            return View(model);
        }


        // ==========================================================
        // Pending Feedback Report
        // ==========================================================

        [HttpGet]
        public async Task<IActionResult> PendingFeedback(
            string? searchText,
            string? region,
            DateTime? fromDate,
            DateTime? toDate,
            long? uploadId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize != 10 &&
                pageSize != 25 &&
                pageSize != 50 &&
                pageSize != 100)
            {
                pageSize = 10;
            }

            var model = await _service.GetPendingFeedbackAsync(
                searchText,
                region,
                fromDate,
                toDate,
                uploadId,
                pageNumber,
                pageSize);

            await _logService.InformationAsync(
                "Reports",
                "PendingFeedback",
                "Viewed Pending Feedback Report.");

            return View(model);
        }
    }
}