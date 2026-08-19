using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IAdminDashboardService _dashboardService;
        private readonly ILogService _logService;

        public DashboardController(
      IAdminDashboardService dashboardService,
      ILogService logService)
        {
            _dashboardService = dashboardService;
            _logService = logService;
        }

        public async Task<IActionResult> Index()
        {
            var departmentClaim = User.FindFirst("DepartmentId")?.Value;

            if (!int.TryParse(departmentClaim, out int departmentId))
            {
                departmentId = 1;
            }

            var model = await _dashboardService.GetDashboardAsync(departmentId);

            await _logService.InformationAsync(
      "Dashboard",
      "Index",
      $"Opened Dashboard. DepartmentID : {departmentId}");

            return View(model);
        }
    }
}