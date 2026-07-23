using Microsoft.AspNetCore.Http;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Implementations
{
    public class FeedbackStatusService : IFeedbackStatusService
    {
        private readonly IFeedbackStatusRepository _repository;

        public FeedbackStatusService(
            IFeedbackStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<FeedbackStatusUploadViewModel> GetPageAsync()
        {
            return await _repository.GetPageAsync();
        }

        public async Task<List<FeedbackStatusUploadHistoryViewModel>>
    GetUploadDetailsAsync(long uploadHeaderId)
        {
            return await _repository.GetUploadDetailsAsync(uploadHeaderId);
        }

        public async Task DeleteUploadAsync(long uploadHeaderId)
        {
            await _repository.DeleteUploadAsync(uploadHeaderId);
        }
        public async Task<long> UploadAsync(
     IFormFile file,
     string status,
     string uploadedBy)
        {
            return await _repository.UploadAsync(
                        file,
                        status,
                        uploadedBy);
        }

        public async Task<ClosedCaseDetailsViewModel>
    GetClosedCaseDetailsAsync(long uploadId)
        {
            return await _repository.GetClosedCaseDetailsAsync(uploadId);
        }

        public async Task CloseUploadedCasesAsync(
    long uploadId,
    string closedBy)
        {
            await _repository.CloseUploadedCasesAsync(
                uploadId,
                closedBy);
        }
        public async Task DeleteAsync(long uploadId)
        {
            await _repository.DeleteAsync(uploadId);
        }

        public async Task<ClosedCaseDetailsViewModel> GetRegionDetailsAsync(
    long uploadId,
    string region)
        {
            return await _repository.GetRegionDetailsAsync(uploadId, region);
        }

        public async Task CloseRegionCasesAsync(
    long uploadId,
    string region,
    string closedBy)
        {
            await _repository.CloseRegionCasesAsync(
                uploadId,
                region,
                closedBy);
        }


    }
}