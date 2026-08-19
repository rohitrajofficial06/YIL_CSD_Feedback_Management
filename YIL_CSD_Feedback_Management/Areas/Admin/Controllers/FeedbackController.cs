using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ILogService _logService;

        public FeedbackController(
      IFeedbackService feedbackService,
      ILogService logService)
        {
            _feedbackService = feedbackService;
            _logService = logService;
        }

        public async Task<IActionResult> Index(FeedbackIndexViewModel model)
        {
            model.DepartmentId = int.Parse(User.FindFirst("DepartmentId")!.Value);

            model = await _feedbackService.GetFeedbackAsync(model);

            await _logService.InformationAsync(
     "Feedback",
     "Index",
     "Viewed feedback list.");

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

            await _logService.InformationAsync(
     "Feedback",
     "Details",
     $"Viewed Feedback Details. FeedbackID : {id}");
                
            return View(model);
        }

        public async Task<IActionResult> ViewAttachment(long id)
        {
            var departmentClaim =
                User.FindFirst("DepartmentId")?.Value;

            if (!int.TryParse(departmentClaim, out int departmentId))
            {
                return Unauthorized();
            }

            var model =
                await _feedbackService.GetAttachmentAsync(
                    id,
                    departmentId);

            if (model == null ||
                string.IsNullOrWhiteSpace(model.FeedbackFilePath))
            {
                return NotFound("Feedback attachment not found.");
            }

            string filePath =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    model.FeedbackFilePath.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Attachment file not found.");
            }

            string contentType =
                GetContentType(model.FeedbackFileName);

            await _logService.InformationAsync(
                "Feedback",
                "ViewAttachment",
                $"Viewed Feedback Attachment. FeedbackID : {id}");

            return PhysicalFile(
                filePath,
                contentType,
                enableRangeProcessing: true);
        }

        private string GetContentType(string? fileName)
        {
            string extension =
                Path.GetExtension(fileName ?? "")
                    .ToLowerInvariant();

            return extension switch
            {
                ".pdf" => "application/pdf",

                ".jpg" => "image/jpeg",

                ".jpeg" => "image/jpeg",

                ".png" => "image/png",

                _ => "application/octet-stream"
            };
        }


    }
}