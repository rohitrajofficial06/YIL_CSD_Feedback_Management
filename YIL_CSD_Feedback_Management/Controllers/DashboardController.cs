using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Controllers
{
    [Authorize(Roles = "User,Manager")]
    public class DashboardController : Controller
    {
        private readonly IEngineerDashboardService _service;
        private readonly ILogService _logService;

        public DashboardController(
            IEngineerDashboardService service,
            ILogService logService)
        {
            _service = service;
            _logService = logService;
        }

        public async Task<IActionResult> Index()
        {
            string createdBy = User.Identity?.Name ?? "";

            string fullName = User.FindFirst("FullName")?.Value ?? "";

            string region = User.FindFirst("Region")?.Value ?? "";

            string module = User.FindFirst("Module")?.Value ?? "";

            int departmentId = Convert.ToInt32(
                User.FindFirst("DepartmentId")?.Value ?? "0");

            var model = await _service.GetDashboardAsync(
                createdBy,
                fullName,
                region,
                module,
                departmentId);

            await _logService.InformationAsync(
                "Dashboard",
                "Index",
                $"{createdBy} opened Dashboard.");

            return View(model);
        }
    }
}