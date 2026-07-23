using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ClosedCasesController : Controller
    {
        private readonly IClosedCasesService _service;

        public ClosedCasesController(IClosedCasesService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(
    ClosedCasesViewModel filter)
        {
            var model =
                await _service.GetPageAsync(filter);

            return View(model);
        }
    }
}