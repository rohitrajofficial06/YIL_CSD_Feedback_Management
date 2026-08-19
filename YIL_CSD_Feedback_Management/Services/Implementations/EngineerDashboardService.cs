using YIL_CSD_Feedback_Management.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Services.Interfaces;
using YIL_CSD_Feedback_Management.ViewModels.Dashboard;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class EngineerDashboardService : IEngineerDashboardService
    {
        private readonly IEngineerDashboardRepository _repository;

        public EngineerDashboardService(
            IEngineerDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<EngineerDashboardViewModel> GetDashboardAsync(
            string createdBy,
            string fullName,
            string region,
            string module,
            int departmentId)
        {
            return await _repository.GetDashboardAsync(
                createdBy,
                fullName,
                region,
                module,
                departmentId);
        }
    }
}