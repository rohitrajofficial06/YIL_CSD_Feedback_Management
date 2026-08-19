using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Helpers;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.ViewModels.MyFeedback;

namespace YIL_CSD_Feedback_Management.Repositories.Implementations
{
    public class MyFeedbackRepository : IMyFeedbackRepository
    {
        private readonly ApplicationDbContext _context;

        public MyFeedbackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //----------------------------------------------------
        // My Feedback List
        //----------------------------------------------------

        public async Task<(List<CustomerFeedback> Feedbacks, int TotalRecords)> GetMyFeedbackAsync(
     string createdBy,
     string? searchText,
     string? status,
     string? region,
     DateTime? fromDate,
     DateTime? toDate,
     int pageNumber,
     int pageSize,
     string sortColumn,
     string sortOrder)
        {
            var query = _context.CustomerFeedbacks
                .Include(x => x.Ratings)
                .Where(x => x.CreatedBy == createdBy && x.IsActive);

            //---------------------------------------
            // Search
            //---------------------------------------

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(x =>
                    x.CaseNumber!.Contains(searchText) ||
                    x.CompanyName.Contains(searchText) ||
                    x.RespondentName.Contains(searchText));
            }

            //---------------------------------------
            // Status
            //---------------------------------------

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.FeedbackStatus == status);
            }

            //---------------------------------------
            // Region
            //---------------------------------------

            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(x => x.Region == region);
            }

            //---------------------------------------
            // Date
            //---------------------------------------

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate >= fromDate);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate < toDate.Value.AddDays(1));
            }

            //---------------------------------------
            // Total Records
            //---------------------------------------

            int totalRecords = await query.CountAsync();

            //---------------------------------------
            // Sorting
            //---------------------------------------

            switch (sortColumn)
            {
                case "CaseNumber":

                    query = sortOrder == "asc"
                        ? query.OrderBy(x => x.CaseNumber)
                        : query.OrderByDescending(x => x.CaseNumber);

                    break;

                case "Company":

                    query = sortOrder == "asc"
                        ? query.OrderBy(x => x.CompanyName)
                        : query.OrderByDescending(x => x.CompanyName);

                    break;

                case "Region":

                    query = sortOrder == "asc"
                        ? query.OrderBy(x => x.Region)
                        : query.OrderByDescending(x => x.Region);

                    break;

                case "Status":

                    query = sortOrder == "asc"
                        ? query.OrderBy(x => x.FeedbackStatus)
                        : query.OrderByDescending(x => x.FeedbackStatus);

                    break;

                default:

                    query = sortOrder == "asc"
                        ? query.OrderBy(x => x.CreatedDate)
                        : query.OrderByDescending(x => x.CreatedDate);

                    break;
            }

            //---------------------------------------
            // Pagination
            //---------------------------------------

            var feedbacks = await query

                .Skip((pageNumber - 1) * pageSize)

                .Take(pageSize)

                .ToListAsync();

            return (feedbacks, totalRecords);
        }

        //----------------------------------------------------
        // Feedback Details
        //----------------------------------------------------

        public async Task<MyFeedbackDetailsViewModel?> GetDetailsAsync(
            long feedbackId,
            string createdBy)
        {
            return await _context.CustomerFeedbacks

                .Where(x =>
                    x.FeedbackID == feedbackId &&
                    x.CreatedBy == createdBy &&
                    x.IsActive)

                .Select(x => new MyFeedbackDetailsViewModel
                {
                    FeedbackID = x.FeedbackID,

                    DepartmentID = x.DepartmentID,

                    CaseNumber = x.CaseNumber,

                    CompanyName = x.CompanyName,

                    RespondentName = x.RespondentName,

                    Designation = x.Designation,

                    ContactNo = x.ContactNo,

                    EmailID = x.EmailID,

                    ServiceRequestNo = x.ServiceRequestNo,

                    InstrumentCategory = x.InstrumentCategory,

                    YILEngineer = x.YILEngineer,

                    Region = x.Region,

                    Comments = x.Comments,

                    FeedbackStatus = x.FeedbackStatus,

                    CanEdit = x.FeedbackStatus == "Open"
                              && x.CreatedDate.AddMinutes(30) >= DateTimeHelper.Now,

                    SignaturePath = x.SignaturePath,

                    CreatedDate = x.CreatedDate,

                    Questions = x.Ratings

                        .OrderBy(r => r.QuestionNo)

                        .Select(r => new MyFeedbackQuestionViewModel
                        {
                            QuestionNo = r.QuestionNo,

                            QuestionText = r.QuestionText,

                            RatingValue = r.RatingValue,

                            IsNotApplicable = r.IsNotApplicable
                        })

                        .ToList()
                })

                .FirstOrDefaultAsync();
        }

        public async Task<CustomerFeedback?> GetFeedbackForEditAsync(
    long feedbackId,
    string createdBy)
        {
            return await _context.CustomerFeedbacks

                .Include(x => x.Ratings)

                .FirstOrDefaultAsync(x =>

                    x.FeedbackID == feedbackId

                    && x.CreatedBy == createdBy

                    && x.IsActive

                    && x.FeedbackStatus == "Open"

                 && x.CreatedDate.AddMinutes(30) >= DateTimeHelper.Now);
        }

        public async Task UpdateAsync(CustomerFeedback feedback)
        {
            _context.CustomerFeedbacks.Update(feedback);

            await _context.SaveChangesAsync();
        }

    }
}