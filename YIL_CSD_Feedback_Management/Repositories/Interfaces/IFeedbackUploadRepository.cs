using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Repositories.Interfaces
{
    public interface IFeedbackUploadRepository
    {
        Task<IEnumerable<FeedbackUpload>> GetAllAsync();

        Task<FeedbackUpload?> GetByIdAsync(long id);

        Task AddAsync(FeedbackUpload upload);

        Task UpdateAsync(FeedbackUpload upload);

        Task DeleteAsync(FeedbackUpload upload);

        Task SaveAsync();
    }
}