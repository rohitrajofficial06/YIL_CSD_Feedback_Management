using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<QuestionIndexViewModel> GetQuestionsAsync(QuestionIndexViewModel model)
        {
            var query = _context.FeedbackQuestions
                                .AsNoTracking()
                                .Where(x => !x.IsDeleted)
                                .AsQueryable();

            //==========================================
            // Search
            //==========================================

            if (!string.IsNullOrWhiteSpace(model.SearchText))
            {
                string search = model.SearchText.Trim();

                query = query.Where(x =>
                    x.QuestionText.Contains(search) ||
                    x.QuestionNo.ToString().Contains(search));
            }

            //==========================================
            // Department
            //==========================================

            if (model.DepartmentID.HasValue)
            {
                query = query.Where(x =>
                    x.DepartmentID == model.DepartmentID.Value);
            }


            model.TotalRecords = await query.CountAsync();

            //==========================================
            // Sorting
            //==========================================

            switch (model.SortColumn)
            {
                case "QuestionNo":

                    query = model.SortDirection == "asc"
                        ? query.OrderBy(x => x.QuestionNo)
                        : query.OrderByDescending(x => x.QuestionNo);

                    break;

                case "Department":

                    query = model.SortDirection == "asc"
                        ? query.OrderBy(x => x.Department!.DepartmentName)
                        : query.OrderByDescending(x => x.Department!.DepartmentName);

                    break;

                default:

                    query = model.SortDirection == "asc"
                        ? query.OrderBy(x => x.DisplayOrder)
                        : query.OrderByDescending(x => x.DisplayOrder);

                    break;
            }

            //==========================================
            // Grid
            //==========================================

            model.Questions = await query

       .Skip((model.PageNumber - 1) * model.PageSize)

       .Take(model.PageSize)

       .Select(x => new QuestionListViewModel
       {
           QuestionID = x.QuestionID,

           DepartmentID = x.DepartmentID,

           DepartmentName = x.Department != null
               ? x.Department.DepartmentName
               : "",

           QuestionNo = x.QuestionNo,

           QuestionText = x.QuestionText,

           ControlType = x.ControlType,

           MaxRating = x.MaxRating,

           MinRating = x.MinRating,

           IsMandatory = x.IsMandatory,

           DisplayOrder = x.DisplayOrder
       })

       .ToListAsync();

            return model;
        }

        public async Task AddAsync(FeedbackQuestion question)
        {
            await _context.FeedbackQuestions.AddAsync(question);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<FeedbackQuestion?> GetByIdAsync(int id)
        {
            return await _context.FeedbackQuestions
                .FirstOrDefaultAsync(x => x.QuestionID == id);
        }

        public Task UpdateAsync(FeedbackQuestion question)
        {
            _context.FeedbackQuestions.Update(question);

            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var question = await _context.FeedbackQuestions
                .FirstOrDefaultAsync(x => x.QuestionID == id);

            if (question == null)
                return;

            question.IsDeleted = true;
        }
    }
}