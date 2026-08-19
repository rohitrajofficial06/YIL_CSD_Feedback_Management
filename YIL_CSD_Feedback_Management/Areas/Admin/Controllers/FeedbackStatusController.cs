using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class FeedbackStatusController : Controller
    {
        private readonly IFeedbackStatusService _service;
        private readonly ILogService _logService;

        public FeedbackStatusController(
     IFeedbackStatusService service,
     ILogService logService)
        {
            _service = service;
            _logService = logService;
        }

        //----------------------------------------------------
        // Upload Page
        //----------------------------------------------------

        public async Task<IActionResult> Index()
        {
            var model = await _service.GetPageAsync();

            await _logService.InformationAsync(
    "Feedback Status",
    "Index",
    "Viewed Closed Case Management page.");

            return View(model);
        }

        //----------------------------------------------------
        // Upload Excel
        //----------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(
            FeedbackStatusUploadViewModel model)
        {

            if (model.ExcelFile == null)
            {
                await _logService.WarningAsync(
                    "Feedback Status",
                    "Upload",
                    "Upload attempted without selecting a file.");

                TempData["Error"] = "Please select an Excel file.";

                return RedirectToAction(nameof(Index));
            }

            if (!string.Equals(
                    Path.GetExtension(model.ExcelFile.FileName),
                    ".xlsx",
                    StringComparison.OrdinalIgnoreCase))
            {
                await _logService.WarningAsync(
                    "Feedback Status",
                    "Upload",
                    $"Invalid file uploaded : {model.ExcelFile.FileName}");

                TempData["Error"] = "Only Excel (.xlsx) files are allowed.";

                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await _logService.WarningAsync(
    "Feedback Status",
    "Upload",
    "Upload failed due to validation.");

                model = await _service.GetPageAsync();

                return View("Index", model);
            }

            long uploadId = await _service.UploadAsync(
                      model.ExcelFile,
                      model.SelectedStatus,
                      User.Identity?.Name ?? "Admin");

            await _logService.InformationAsync(
    "Feedback Status",
    "Upload",
    $"Excel uploaded successfully. UploadID : {uploadId}, Status : {model.SelectedStatus}, File : {model.ExcelFile.FileName}");

            return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = uploadId
                    });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);

            await _logService.WarningAsync(
    "Feedback Status",
    "Delete",
    $"Deleted Closed Case Record. UploadID : {id}");

            TempData["Success"] = "Record deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(long id)
        {
            var model =
                await _service.GetClosedCaseDetailsAsync(id);

            await _logService.InformationAsync(
    "Feedback Status",
    "Details",
    $"Viewed Upload Details. UploadID : {id}");

            return View(model);
        }

        public async Task<IActionResult> DeleteUpload(long id)
        {
            await _service.DeleteUploadAsync(id);

            await _logService.WarningAsync(
    "Feedback Status",
    "DeleteUpload",
    $"Deleted Upload. UploadID : {id}");    

            TempData["Success"] =
                "Upload deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseUploadedCases(long id)
        {
            await _service.CloseUploadedCasesAsync(
                id,
                User.Identity?.Name ?? "Admin");

            await _logService.InformationAsync(
    "Feedback Status",
    "CloseUploadedCases",
    $"Closed all uploaded cases. UploadID : {id}");

            TempData["Success"] =
                "Uploaded cases have been processed successfully.";

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> RegionDetails(
    long uploadId,
    string region)
        {
            var model =
                await _service.GetRegionDetailsAsync(uploadId, region);

            await _logService.InformationAsync(
    "Feedback Status",
    "RegionDetails",
    $"Viewed Region Details. UploadID : {uploadId}, Region : {region}");

            ViewBag.Region = region;

            return View(model);
        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CloseRegion(
    long uploadId,
    string region)
        {
            await _service.CloseRegionCasesAsync(
                uploadId,
                region,
                User.Identity?.Name ?? "Admin");

            await _logService.InformationAsync(
    "Feedback Status",
    "CloseRegion",
    $"Closed Region Cases. UploadID : {uploadId}, Region : {region}");

            TempData["Success"] =
                $"{region} cases closed successfully.";

            return RedirectToAction(
                nameof(RegionDetails),
                new
                {
                    uploadId,
                    region
                });
        }



    }
}