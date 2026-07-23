using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class CriticalFeedbackService : ICriticalFeedbackService
    {
        private readonly ICriticalFeedbackRepository _repository;

        public CriticalFeedbackService(
            ICriticalFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task<CriticalFeedbackIndexViewModel> GetAllAsync(
            CriticalFeedbackIndexViewModel model)
        {
            return await _repository.GetAllAsync(model);
        }
    }
}