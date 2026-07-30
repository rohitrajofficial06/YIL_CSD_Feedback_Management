using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Implementations
{
    public class ServiceFeedbackAnalyticsService : IServiceFeedbackAnalyticsService
    {
        private readonly IServiceFeedbackAnalyticsRepository _repository;

        public ServiceFeedbackAnalyticsService(
            IServiceFeedbackAnalyticsRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceFeedbackAnalyticsViewModel> GetAnalyticsAsync(
            ServiceFeedbackAnalyticsViewModel model)
        {
            return await _repository.GetAnalyticsAsync(model);
        }

        public async Task<FileResult> ExportToExcelAsync(int? month, int? year)
        {
            return await _repository.ExportToExcelAsync(month, year);
        }

        public async Task<List<RegionDetailsViewModel>> GetRegionDetailsAsync(
    string region,
    int? month,
    int? year)
        {
            return await _repository.GetRegionDetailsAsync(region, month, year);
        }
    }
}