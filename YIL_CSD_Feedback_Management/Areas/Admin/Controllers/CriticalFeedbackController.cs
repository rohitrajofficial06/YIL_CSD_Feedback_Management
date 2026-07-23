using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CriticalFeedbackController : Controller
    {
        private readonly ICriticalFeedbackService _service;

        public CriticalFeedbackController(
            ICriticalFeedbackService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(
            CriticalFeedbackIndexViewModel model)
        {
            var result = await _service.GetAllAsync(model);

            return View(result);
        }
    }
}