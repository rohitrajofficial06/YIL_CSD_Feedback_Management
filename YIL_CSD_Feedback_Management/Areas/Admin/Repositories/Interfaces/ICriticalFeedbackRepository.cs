using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Repositories.Interfaces
{
    public interface ICriticalFeedbackRepository
    {
        Task<CriticalFeedbackIndexViewModel> GetAllAsync(
            CriticalFeedbackIndexViewModel model);
    }
}