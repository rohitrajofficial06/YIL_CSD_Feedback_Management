using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces
{
    public interface IServiceFeedbackAnalyticsService
    {
        Task<ServiceFeedbackAnalyticsViewModel> GetAnalyticsAsync(
            ServiceFeedbackAnalyticsViewModel model);

        Task<FileResult> ExportToExcelAsync(int? month, int? year);

        Task<List<RegionDetailsViewModel>> GetRegionDetailsAsync(
    string region,
    int? month,
    int? year);
    }
}
