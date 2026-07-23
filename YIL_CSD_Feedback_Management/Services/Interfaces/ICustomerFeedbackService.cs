using YIL_CSD_Feedback_Management.ViewModels;

namespace YIL_CSD_Feedback_Management.Services.Interfaces
{
    public interface ICustomerFeedbackService
    {
        Task<long> SaveAsync(CustomerFeedbackViewModel model);
    }
}