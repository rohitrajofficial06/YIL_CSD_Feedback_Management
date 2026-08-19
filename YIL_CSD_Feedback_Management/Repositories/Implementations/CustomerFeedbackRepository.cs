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

        //----------------------------------------------------
        // Add Feedback
        //----------------------------------------------------

        public async Task AddAsync(CustomerFeedback feedback)
        {
            await _context.CustomerFeedbacks.AddAsync(feedback);
        }

        //----------------------------------------------------
        // Save Changes
        //----------------------------------------------------

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        //----------------------------------------------------
        // Validate Case Number
        //----------------------------------------------------

        public async Task<bool> CaseNumberExistsAsync(string caseNumber)
        {
            return await _context.CustomerFeedbacks
                .AnyAsync(x =>
                    x.CaseNumber == caseNumber &&
                    x.IsActive);
        }

        //----------------------------------------------------
        // Get Feedback For Edit
        //----------------------------------------------------

        public async Task<CustomerFeedback?> GetForEditAsync(
     long feedbackId,
     string createdBy)
        {
            return await _context.CustomerFeedbacks
                .Include(x => x.Ratings)
                .FirstOrDefaultAsync(x =>
                    x.FeedbackID == feedbackId &&
                    x.CreatedBy == createdBy &&
                    x.IsActive &&
                    x.FeedbackStatus == "Open" &&
                    x.CreatedDate.AddMinutes(30) >= DateTime.Now);
        }

        //----------------------------------------------------
        // Update Feedback
        //----------------------------------------------------

        public async Task UpdateAsync(CustomerFeedback feedback)
        {
            _context.CustomerFeedbacks.Update(feedback);

            await _context.SaveChangesAsync();
        }

        //----------------------------------------------------
        // My Feedback
        //----------------------------------------------------

        public async Task<List<CustomerFeedback>> GetMyFeedbackAsync(
            string createdBy,
            string? searchText,
            string? status,
            string? region,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.CustomerFeedbacks
                .Include(x => x.Ratings)
                .Where(x => x.CreatedBy == createdBy);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(x =>
                    x.CaseNumber!.Contains(searchText) ||
                    x.CompanyName.Contains(searchText) ||
                    x.RespondentName.Contains(searchText));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.FeedbackStatus == status);
            }

            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(x => x.Region == region);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate >= fromDate);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate <= toDate.Value.AddDays(1));
            }

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }
    }
}