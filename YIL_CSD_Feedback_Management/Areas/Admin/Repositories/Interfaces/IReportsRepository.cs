using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces
{
    public interface IReportsRepository
    {
        Task<ReportsDashboardViewModel> GetDashboardAsync();

        Task<PendingFeedbackViewModel> GetPendingFeedbackAsync(
            string? searchText,
            string? region,
            DateTime? fromDate,
            DateTime? toDate,
            long? uploadId,
            int pageNumber,
            int pageSize);
    }
}