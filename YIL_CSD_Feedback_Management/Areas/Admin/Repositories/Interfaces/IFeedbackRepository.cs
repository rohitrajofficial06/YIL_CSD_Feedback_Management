using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<FeedbackIndexViewModel> GetFeedbackAsync(FeedbackIndexViewModel model);

        Task<FeedbackDetailsViewModel?> GetDetailsAsync(long feedbackId);
    }
}