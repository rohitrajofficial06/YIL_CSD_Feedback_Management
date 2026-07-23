using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class FeedbackQuestionService : IFeedbackQuestionService
    {
        private readonly IFeedbackQuestionRepository _repository;

        public FeedbackQuestionService(IFeedbackQuestionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<FeedbackQuestion>> GetByDepartmentAsync(int departmentId)
        {
            return await _repository.GetByDepartmentAsync(departmentId);
        }
    }
}