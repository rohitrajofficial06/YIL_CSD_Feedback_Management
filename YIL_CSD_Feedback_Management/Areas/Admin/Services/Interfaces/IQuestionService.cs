using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionIndexViewModel> GetQuestionsAsync(QuestionIndexViewModel model);
        Task CreateAsync(QuestionCreateViewModel model);

        Task<QuestionCreateViewModel?> GetByIdAsync(int id);

        Task UpdateAsync(QuestionCreateViewModel model);

        Task DeleteAsync(int id);
    }
}