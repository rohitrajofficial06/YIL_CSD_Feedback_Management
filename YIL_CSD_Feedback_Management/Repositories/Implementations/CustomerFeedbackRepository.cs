using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;

namespace YIL_CSD_Feedback_Management.Repositories.Implementations
{
    public class CustomerFeedbackRepository : ICustomerFeedbackRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerFeedbackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CustomerFeedback feedback)
        {
            feedback.CaseNumber = await GenerateCaseNumberAsync(feedback.DepartmentID);

            await _context.CustomerFeedbacks.AddAsync(feedback);
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        private async Task<string> GenerateCaseNumberAsync(int departmentId)
        {
            string prefix = $"YIL-C{departmentId:D4}-";

            string? lastCaseNumber = await _context.CustomerFeedbacks
                .Where(x => x.DepartmentID == departmentId)
                .OrderByDescending(x => x.FeedbackID)
                .Select(x => x.CaseNumber)
                .FirstOrDefaultAsync();

            int nextSequence = 1;

            if (!string.IsNullOrWhiteSpace(lastCaseNumber))
            {
                string[] parts = lastCaseNumber.Split('-');

                if (parts.Length == 3 &&
                    int.TryParse(parts[2], out int last))
                {
                    nextSequence = last + 1;
                }
            }

            return $"{prefix}{nextSequence:D5}";
        }

    }
}