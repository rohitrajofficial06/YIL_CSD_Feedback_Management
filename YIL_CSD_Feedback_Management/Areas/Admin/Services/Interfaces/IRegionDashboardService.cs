using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces
{
    public interface IRegionDashboardService
    {
        Task<RegionDashboardViewModel> GetDashboardAsync();

        Task<ClosedCasesViewModel> GetRegionCasesAsync(string region);
    }
}