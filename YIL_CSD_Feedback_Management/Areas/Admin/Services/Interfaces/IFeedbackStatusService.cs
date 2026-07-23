using Microsoft.AspNetCore.Http;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces
{
    public interface IFeedbackStatusService
    {
        Task<FeedbackStatusUploadViewModel> GetPageAsync();

        Task<long> UploadAsync(
            IFormFile file,
            string status,
            string uploadedBy);

        Task DeleteAsync(long uploadId);

        Task<ClosedCaseDetailsViewModel>
    GetClosedCaseDetailsAsync(long uploadId);

        Task CloseUploadedCasesAsync(long uploadId, string closedBy);


        Task<List<FeedbackStatusUploadHistoryViewModel>>
    GetUploadDetailsAsync(long uploadHeaderId);

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