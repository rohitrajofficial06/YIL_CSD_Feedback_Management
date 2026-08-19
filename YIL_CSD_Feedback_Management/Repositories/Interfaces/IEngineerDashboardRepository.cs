using YIL_CSD_Feedback_Management.ViewModels.Dashboard;

namespace YIL_CSD_Feedback_Management.Repositories.Interfaces
{
    public interface IEngineerDashboardRepository
    {
        Task<EngineerDashboardViewModel> GetDashboardAsync(
            string createdBy,
            string fullName,
            string region,
            string module,
            int departmentId);
    }
}