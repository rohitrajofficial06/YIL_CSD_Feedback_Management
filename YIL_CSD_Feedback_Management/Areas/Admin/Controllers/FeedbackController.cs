using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public async Task<IActionResult> Index(FeedbackIndexViewModel model)
        {
            model.DepartmentId = int.Parse(User.FindFirst("DepartmentId")!.Value);

            model = await _feedbackService.GetFeedbackAsync(model);

            return View(model);
        }

        public async Task<IActionResult> Details(long id)
        {
            var departmentClaim = User.FindFirst("DepartmentId")?.Value;

            if (!int.TryParse(departmentClaim, out int departmentId))
            {
                return Unauthorized();
            }

            var model = await _feedbackService.GetDetailsAsync(id, departmentId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


    }
}