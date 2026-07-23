using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;

namespace YIL_CSD_Feedback_Management.Repositories.Implementations
{
    public class FeedbackQuestionRepository : IFeedbackQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public FeedbackQuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FeedbackQuestion>> GetByDepartmentAsync(int departmentId)
        {
            return await _context.FeedbackQuestions
                .Where(x => x.DepartmentID == departmentId &&
                            x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }
    }
}