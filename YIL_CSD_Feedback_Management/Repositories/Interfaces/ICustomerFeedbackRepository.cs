using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Repositories.Interfaces
{
    public interface ICustomerFeedbackRepository
    {
        Task AddAsync(CustomerFeedback feedback);

        Task SaveAsync();

        Task<bool> CaseNumberExistsAsync(string caseNumber);

        Task<List<CustomerFeedback>> GetMyFeedbackAsync(
            string createdBy,
            string? searchText,
            string? status,
            string? region,
            DateTime? fromDate,
            DateTime? toDate);

        Task<CustomerFeedback?> GetForEditAsync(
            long feedbackId,
            string createdBy);

        // THIS MUST EXIST
        Task UpdateAsync(CustomerFeedback feedback);
    }
}