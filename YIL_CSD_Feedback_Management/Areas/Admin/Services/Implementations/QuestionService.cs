using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repository;

        public QuestionService(IQuestionRepository repository)
        {
            _repository = repository;
        }

        public async Task<QuestionIndexViewModel> GetQuestionsAsync(QuestionIndexViewModel model)
        {
            return await _repository.GetQuestionsAsync(model);
        }

        public async Task CreateAsync(QuestionCreateViewModel model)
        {
            FeedbackQuestion question = new FeedbackQuestion
            {
                DepartmentID = model.DepartmentID,

                QuestionNo = model.QuestionNo,

                QuestionText = model.QuestionText,

                ControlType = model.ControlType,

                MinRating = model.MinRating,

                MaxRating = model.MaxRating,

                IsMandatory = model.IsMandatory,

                DisplayOrder = model.DisplayOrder
            };

            await _repository.AddAsync(question);

            await _repository.SaveAsync();
        }

        public async Task<QuestionCreateViewModel?> GetByIdAsync(int id)
        {
            var question = await _repository.GetByIdAsync(id);

            if (question == null)
                return null;

            return new QuestionCreateViewModel
            {
                QuestionID = question.QuestionID,

                DepartmentID = question.DepartmentID,

                QuestionNo = question.QuestionNo,

                QuestionText = question.QuestionText,

                ControlType = question.ControlType,

                MinRating = question.MinRating,

                MaxRating = question.MaxRating,

                DisplayOrder = question.DisplayOrder,

                IsMandatory = question.IsMandatory
            };
        }

        public async Task UpdateAsync(QuestionCreateViewModel model)
        {
            var question = await _repository.GetByIdAsync(model.QuestionID);

            if (question == null)
                return;

            question.DepartmentID = model.DepartmentID;

            question.QuestionNo = model.QuestionNo;

            question.QuestionText = model.QuestionText;

            question.ControlType = model.ControlType;

            question.MinRating = model.MinRating;

            question.MaxRating = model.MaxRating;

            question.DisplayOrder = model.DisplayOrder;

            question.IsMandatory = model.IsMandatory;

            await _repository.UpdateAsync(question);

            await _repository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);

            await _repository.SaveAsync();
        }
    }
}