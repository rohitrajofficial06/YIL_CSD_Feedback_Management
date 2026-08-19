using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Implementations
{
    public class ReportsService : IReportsService
    {
        private readonly IReportsRepository _repository;

        public ReportsService(IReportsRepository repository)
        {
            _repository = repository;
        }

        // ==========================================================
        // Reports Dashboard
        // ==========================================================

        public async Task<ReportsDashboardViewModel> GetDashboardAsync()
        {
            return await _repository.GetDashboardAsync();
        }


        // ==========================================================
        // Pending Feedback Report
        // ==========================================================

        public async Task<PendingFeedbackViewModel> GetPendingFeedbackAsync(
            string? searchText,
            string? region,
            DateTime? fromDate,
            DateTime? toDate,
            long? uploadId,
            int pageNumber,
            int pageSize)
        {
            return await _repository.GetPendingFeedbackAsync(
                searchText,
                region,
                fromDate,
                toDate,
                uploadId,
                pageNumber,
                pageSize);
        }
    }
}