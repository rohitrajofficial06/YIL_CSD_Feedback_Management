using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Implementations
{
    public class EngineerPerformanceService : IEngineerPerformanceService
    {
        private readonly IEngineerPerformanceRepository _repository;

        public EngineerPerformanceService(
            IEngineerPerformanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<EngineerPerformanceViewModel> GetDashboardAsync()
        {
            return await _repository.GetDashboardAsync();
        }

        public async Task<EngineerDetailsViewModel> GetEngineerDetailsAsync(
    EngineerDetailsViewModel filter)
        {
            return await _repository.GetEngineerDetailsAsync(filter);
        }
    }
}