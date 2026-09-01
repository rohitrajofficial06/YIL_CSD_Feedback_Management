using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
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

        // ============================================================
        // MAIN ANALYTICS PAGE
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            int? month,
            int? year)
        {
            DateTime now = DateTime.Now;

            int selectedMonth =
                month ?? now.Month;

            int selectedYear =
                year ?? now.Year;

            // --------------------------------------------------------
            // Get analytics for selected month/year
            // --------------------------------------------------------

            var model =
                await _service.GetAnalyticsAsync(
                    selectedMonth,
                    selectedYear);

            // --------------------------------------------------------
            // Log
            // --------------------------------------------------------

            await _logService.InformationAsync(
                "Feedback Analytics",
                "Index",
                $"Viewed Feedback Analytics for {selectedMonth}/{selectedYear}.");

            return View(model);
        }


        // ============================================================
        // REGION DETAILS / PENDING FEEDBACK
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> RegionDetails(
            string region,
            int? month,
            int? year)
        {
            // --------------------------------------------------------
            // Region is required
            // --------------------------------------------------------

            if (string.IsNullOrWhiteSpace(region))
            {
                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        month = month,
                        year = year
                    });
            }

            DateTime now = DateTime.Now;

            int selectedMonth =
                month ?? now.Month;

            int selectedYear =
                year ?? now.Year;

            // --------------------------------------------------------
            // Get region details
            //
            // Repository uses:
            // tblClosedCaseUploadDetail
            // ExcelClosedDate
            // CaseNumber
            //
            // and matches against:
            // trnCustomerFeedback
            // --------------------------------------------------------

            var model =
                await _service.GetRegionDetailsAsync(
                    region.Trim(),
                    selectedMonth,
                    selectedYear);

            // --------------------------------------------------------
            // Pass filter information to View
            // --------------------------------------------------------

            ViewBag.Region =
                region.Trim();

            ViewBag.Month =
                selectedMonth;

            ViewBag.Year =
                selectedYear;

            // --------------------------------------------------------
            // Log
            // --------------------------------------------------------

            await _logService.InformationAsync(
                "Feedback Analytics",
                "RegionDetails",
                $"Viewed {region.Trim()} region details for {selectedMonth}/{selectedYear}.");

            return View(model);
        }


        // ============================================================
        // EXPORT TO EXCEL
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Export(
            int? month,
            int? year)
        {
            // --------------------------------------------------------
            // Export selected month/year
            // --------------------------------------------------------

            var file =
                await _service.ExportToExcelAsync(
                    month,
                    year);

            // --------------------------------------------------------
            // Log
            // --------------------------------------------------------

            await _logService.InformationAsync(
                "Feedback Analytics",
                "Export",
                $"Exported Feedback Analytics for {month}/{year}.");

            return file;
        }
    }
}