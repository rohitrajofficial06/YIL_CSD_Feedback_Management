using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Implementations
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<FeedbackIndexViewModel> GetFeedbackAsync(FeedbackIndexViewModel model)
        {
            return await _feedbackRepository.GetFeedbackAsync(model);
        }

        public async Task<FeedbackDetailsViewModel?> GetDetailsAsync(long feedbackId, int departmentId)
        {
            return await _feedbackRepository.GetDetailsAsync(feedbackId, departmentId);
        }
    }
}