using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<FeedbackIndexViewModel> GetFeedbackAsync(FeedbackIndexViewModel model);
        Task<FeedbackDetailsViewModel?> GetDetailsAsync(long feedbackId, int departmentId);
    }
}