using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class FeedbackStatusController : Controller
    {
        private readonly IFeedbackStatusService _service;

        public FeedbackStatusController(
            IFeedbackStatusService service)
        {
            _service = service;
        }

        //----------------------------------------------------
        // Upload Page
        //----------------------------------------------------

        public async Task<IActionResult> Index()
        {
            var model = await _service.GetPageAsync();

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

            if (Path.GetExtension(model.ExcelFile.FileName).ToLower() != ".xlsx")
            {
                TempData["Error"] = "Only Excel (.xlsx) files are allowed.";

                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model = await _service.GetPageAsync();

                return View("Index", model);
            }

            long uploadId = await _service.UploadAsync(
                      model.ExcelFile,
                      model.SelectedStatus,
                      User.Identity?.Name ?? "Admin");

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

            TempData["Success"] = "Record deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(long id)
        {
            var model =
                await _service.GetClosedCaseDetailsAsync(id);

            return View(model);
        }

        public async Task<IActionResult> DeleteUpload(long id)
        {
            await _service.DeleteUploadAsync(id);

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