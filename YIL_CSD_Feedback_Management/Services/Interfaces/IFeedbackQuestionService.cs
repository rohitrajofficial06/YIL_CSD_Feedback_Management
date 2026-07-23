using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Services.Interfaces
{
    public interface IFeedbackQuestionService
    {
        Task<IEnumerable<FeedbackQuestion>> GetByDepartmentAsync(int departmentId);
    }
}