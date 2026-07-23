using Microsoft.AspNetCore.Mvc;
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
            model.DepartmentId = 1; // Site Service

            model = await _feedbackService.GetFeedbackAsync(model);

            return View(model);
        }

        public async Task<IActionResult> Details(long id)
        {
            var model = await _feedbackService.GetDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

      
    }
}