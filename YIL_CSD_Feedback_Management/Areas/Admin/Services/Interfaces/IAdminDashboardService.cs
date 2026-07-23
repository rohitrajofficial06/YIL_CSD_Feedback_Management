using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<DashboardViewModel> GetDashboardAsync(int departmentId);
    }
}