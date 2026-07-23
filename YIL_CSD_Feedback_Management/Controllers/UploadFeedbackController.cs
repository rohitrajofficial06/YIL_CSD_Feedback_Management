using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using YIL_CSD_Feedback_Management.Models.ViewModels;
using YIL_CSD_Feedback_Management.Services.Interfaces;


namespace YIL_CSD_Feedback_Management.Controllers
{
    [Authorize]
    public class UploadFeedbackController : Controller
    {
        private readonly IFeedbackUploadService _feedbackUploadService;
        private readonly IDepartmentService _departmentService;
        public UploadFeedbackController(
     IFeedbackUploadService feedbackUploadService,
     IDepartmentService departmentService)
        {
            _feedbackUploadService = feedbackUploadService;
            _departmentService = departmentService;
        }

        // ===========================
        // Upload Page
        // ===========================

        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllAsync();

            FeedbackUploadViewModel model = new FeedbackUploadViewModel
            {
                UploadedFiles = await _feedbackUploadService.GetAllAsync(),

                Departments = departments
                    .Select(x => new SelectListItem
                    {
                        Text = x.DepartmentName,
                        Value = x.DepartmentID.ToString()
                    })
                    .ToList()
            };

            return View(model);
        }

        // ===========================
        // Upload File
        // ===========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(FeedbackUploadViewModel model)
        {
            try
            {
                if (model.FeedbackFile == null || model.FeedbackFile.Length == 0)
                {
                    TempData["Error"] = "Please select a feedback file.";
                    return RedirectToAction(nameof(Index));
                }

                long uploadId = await _feedbackUploadService.UploadAsync(model.FeedbackFile, model.DepartmentID);

                TempData["Success"] = $"Feedback uploaded successfully. Upload ID : {uploadId}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(long id)
        {
            var upload = await _feedbackUploadService.GetByIdAsync(id);

            if (upload == null)
                return NotFound();

            return View(upload);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExtractOCR(long id)
        {
            try
            {
                string extractedText = await _feedbackUploadService.ExtractOCRAsync(id);

                TempData["OCRResult"] = extractedText;

                return RedirectToAction(nameof(OCR), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(nameof(Details), new { id });
            }
        }


        public async Task<IActionResult> OCR(long id)
        {
            var upload = await _feedbackUploadService.GetByIdAsync(id);

            if (upload == null)
                return NotFound();

            return View(upload);
        }

        [HttpPost]
        public async Task<IActionResult> UploadAjax(FeedbackUploadViewModel model)
        {
            try
            {
                if (model.FeedbackFile == null || model.FeedbackFile.Length == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please select a file."
                    });
                }

                long uploadId = await _feedbackUploadService.UploadAsync(
                    model.FeedbackFile,
                    model.DepartmentID);

                return Json(new
                {
                    success = true,
                    uploadId = uploadId,
                    message = "Feedback uploaded successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        public async Task<IActionResult> UploadGrid()
        {
            var uploads = await _feedbackUploadService.GetAllAsync();

            return PartialView("_UploadGrid", uploads);
        }
    }
}