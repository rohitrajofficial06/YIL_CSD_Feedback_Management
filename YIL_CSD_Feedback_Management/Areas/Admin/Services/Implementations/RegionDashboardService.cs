using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Implementations
{
    public class RegionDashboardService : IRegionDashboardService
    {
        private readonly IRegionDashboardRepository _repository;

        public RegionDashboardService(IRegionDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<RegionDashboardViewModel> GetDashboardAsync()
        {
            return await _repository.GetDashboardAsync();
        }

        public async Task<ClosedCasesViewModel> GetRegionCasesAsync(string region)
        {
            return await _repository.GetRegionCasesAsync(region);
        }
    }
}