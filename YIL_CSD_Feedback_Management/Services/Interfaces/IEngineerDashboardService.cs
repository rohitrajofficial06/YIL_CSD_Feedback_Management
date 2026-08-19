using YIL_CSD_Feedback_Management.ViewModels.Dashboard;

namespace YIL_CSD_Feedback_Management.Services.Interfaces
{
    public interface IEngineerDashboardService
    {
        Task<EngineerDashboardViewModel> GetDashboardAsync(
            string createdBy,
            string fullName,
            string region,
            string module,
            int departmentId);
    }
}