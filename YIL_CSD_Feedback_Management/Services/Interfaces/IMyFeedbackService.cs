using YIL_CSD_Feedback_Management.ViewModels.MyFeedback;

namespace YIL_CSD_Feedback_Management.Services.Interfaces
{
    public interface IMyFeedbackService
    {
        Task<MyFeedbackSearchViewModel> GetMyFeedbackAsync(
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

        Task<MyFeedbackDetailsViewModel?> GetDetailsAsync(
            long feedbackId,
            string createdBy);
    }
}