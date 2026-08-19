using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces
{
    public interface IReportsService
    {
        // Reports Dashboard
        Task<ReportsDashboardViewModel> GetDashboardAsync();

        // Pending Feedback Report
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