using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces
{
    public interface IAdminDashboardRepository
    {
        Task<DashboardViewModel> GetDashboardAsync(int departmentId);
    }
}