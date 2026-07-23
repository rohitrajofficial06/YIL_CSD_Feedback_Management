using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportsController : Controller
    {
        private readonly IReportsService _service;

        public ReportsController(IReportsService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _service.GetDashboardAsync();

            return View(model);
        }
    }
}