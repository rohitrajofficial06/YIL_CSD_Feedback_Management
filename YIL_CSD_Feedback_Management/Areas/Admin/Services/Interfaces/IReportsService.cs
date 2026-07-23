using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces
{
    public interface IReportsService
    {
        Task<ReportsDashboardViewModel> GetDashboardAsync();
    }
}