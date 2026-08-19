using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.ViewModels.MyFeedback;

namespace YIL_CSD_Feedback_Management.Repositories.Interfaces
{
    public interface IMyFeedbackRepository
    {
        Task<(List<CustomerFeedback> Feedbacks, int TotalRecords)>
  GetMyFeedbackAsync(
      string createdBy,
      string? searchText,
      string? status,
      string? region,
      DateTime? fromDate,
      DateTime? toDate,
      int pageNumber,
      int pageSize,
      string sortColumn,
      string sortOrder);

        Task<CustomerFeedback?> GetFeedbackForEditAsync(
    long feedbackId,
    string createdBy);

        Task UpdateAsync(CustomerFeedback feedback);

        Task<MyFeedbackDetailsViewModel?> GetDetailsAsync(
            long feedbackId,
            string createdBy);
    }
}