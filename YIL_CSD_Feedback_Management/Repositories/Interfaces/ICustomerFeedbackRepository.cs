using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Repositories.Interfaces
{
    public interface ICustomerFeedbackRepository
    {
        Task AddAsync(CustomerFeedback feedback);

        Task SaveAsync();
    }
}