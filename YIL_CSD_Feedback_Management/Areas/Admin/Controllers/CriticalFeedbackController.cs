using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CriticalFeedbackController : Controller
    {
        private readonly ICriticalFeedbackService _service;
        private readonly ILogService _logService;

        public CriticalFeedbackController(
      ICriticalFeedbackService service,
      ILogService logService)
        {
            _service = service;
            _logService = logService;
        }

        public async Task<IActionResult> Index(
      CriticalFeedbackIndexViewModel model)
        {
            var departmentClaim = User.FindFirst("DepartmentId")?.Value;

            if (int.TryParse(departmentClaim, out int departmentId))
            {
                model.DepartmentId = departmentId;
            }

            var result = await _service.GetAllAsync(model);

            await _logService.InformationAsync(
      "Critical Feedback",
      "Index",
      $"Viewed Critical Feedback List. DepartmentID : {model.DepartmentId}, Region : {model.Region}, Search : {model.SearchText}");

            return View(result);
        }
    }
}