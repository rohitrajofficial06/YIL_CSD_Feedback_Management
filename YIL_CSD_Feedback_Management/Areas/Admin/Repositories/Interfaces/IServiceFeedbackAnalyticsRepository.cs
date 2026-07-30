using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces
{
    public interface IServiceFeedbackAnalyticsRepository
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
