using Microsoft.AspNetCore.Http;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces
{
    public interface IFeedbackStatusRepository
    {
        Task<FeedbackStatusUploadViewModel> GetPageAsync();

        Task<long> UploadAsync(
            IFormFile file,
            string status,
            string uploadedBy);

        Task DeleteAsync(long uploadId);

        Task<List<FeedbackStatusUploadHistoryViewModel>>
            GetUploadDetailsAsync(long uploadHeaderId);

        Task<ClosedCaseDetailsViewModel> GetClosedCaseDetailsAsync(long uploadId);
        Task CloseUploadedCasesAsync(long uploadId, string closedBy);

        Task DeleteUploadAsync(long uploadHeaderId);

        Task<ClosedCaseDetailsViewModel> GetRegionDetailsAsync(
    long uploadId,
    string region);

        Task CloseRegionCasesAsync(
    long uploadId,
    string region,
    string closedBy);



    }


}