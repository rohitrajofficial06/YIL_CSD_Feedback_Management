using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.ViewModels;

namespace YIL_CSD_Feedback_Management.Services.Interfaces
{
    public interface ICustomerFeedbackService
    {

        Task<bool> CaseNumberExistsAsync(string caseNumber);

        Task<CustomerFeedback?> GetForEditAsync(
    long feedbackId,
    string createdBy);

        Task UpdateAsync(CustomerFeedback feedback);
        Task<long> SaveAsync(
    CustomerFeedbackViewModel model,
    string createdBy);
    }
}