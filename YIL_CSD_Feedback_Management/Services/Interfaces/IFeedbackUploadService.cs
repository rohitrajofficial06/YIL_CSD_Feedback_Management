using Microsoft.AspNetCore.Http;
using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Services.Interfaces
{
    public interface IFeedbackUploadService
    {
        Task<long> UploadAsync(IFormFile file, int? departmentId);

        Task<IEnumerable<FeedbackUpload>> GetAllAsync();

        Task<FeedbackUpload?> GetByIdAsync(long id);

        Task<string> ExtractOCRAsync(long uploadId);
    }
}