using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Repositories.Interfaces
{
    public interface IFeedbackQuestionRepository
    {
        Task<IEnumerable<FeedbackQuestion>> GetByDepartmentAsync(int departmentId);
    }
}