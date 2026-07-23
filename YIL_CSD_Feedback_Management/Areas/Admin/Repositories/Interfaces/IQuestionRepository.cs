using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<QuestionIndexViewModel> GetQuestionsAsync(QuestionIndexViewModel model);

        Task AddAsync(FeedbackQuestion question);

        Task<FeedbackQuestion?> GetByIdAsync(int id);

        Task UpdateAsync(FeedbackQuestion question);

        Task DeleteAsync(int id);

        Task SaveAsync();
    }
}