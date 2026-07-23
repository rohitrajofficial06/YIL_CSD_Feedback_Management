using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Services.Interfaces
{
    public interface ICriticalFeedbackService
    {
        Task<CriticalFeedbackIndexViewModel> GetAllAsync(
            CriticalFeedbackIndexViewModel model);
    }
}