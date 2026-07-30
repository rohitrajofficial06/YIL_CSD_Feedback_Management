using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IAdminDashboardService _dashboardService;

        public DashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var departmentClaim = User.FindFirst("DepartmentId")?.Value;

            if (!int.TryParse(departmentClaim, out int departmentId))
            {
                departmentId = 1;
            }

            var model = await _dashboardService.GetDashboardAsync(departmentId);

            return View(model);
        }
    }
}