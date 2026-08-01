using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations
{
    public class CriticalFeedbackRepository : ICriticalFeedbackRepository
    {
        private readonly ApplicationDbContext _context;

        public CriticalFeedbackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CriticalFeedbackIndexViewModel> GetAllAsync(CriticalFeedbackIndexViewModel model)
        {
            IQueryable<CustomerFeedback> feedbackQuery = _context.CustomerFeedbacks
                .Include(x => x.Ratings)
                .AsNoTracking();

            var criticalFeedbackIds = await _context.CustomerFeedbackRatings
     .Where(r => r.RatingValue.HasValue)
     .GroupBy(r => r.FeedbackID)
     .Where(g => g.Average(x => x.RatingValue.Value) <= 3)
     .Select(g => g.Key)
     .ToListAsync();

            feedbackQuery = _context.CustomerFeedbacks
                .Include(x => x.Ratings)
                .Where(x => criticalFeedbackIds.Contains(x.FeedbackID))
                .AsNoTracking();

            //===========================================
            // Department Filter
            //===========================================

            if (model.DepartmentId.HasValue)
            {
                feedbackQuery = feedbackQuery.Where(x =>
                    x.DepartmentID == model.DepartmentId.Value);
            }

            if (!string.IsNullOrWhiteSpace(model.SearchText))
            {
                string search = model.SearchText.Trim();

                feedbackQuery = feedbackQuery.Where(x =>

                    x.CompanyName.Contains(search) ||

                    x.RespondentName.Contains(search) ||

                    (x.ServiceRequestNo != null &&
                     x.ServiceRequestNo.Contains(search)) ||

                    (x.YILEngineer != null &&
                     x.YILEngineer.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(model.Region))
            {
                feedbackQuery = feedbackQuery.Where(x =>
                    _context.ClosedCaseUploadDetails.Any(c =>

                        c.CaseNumber == x.CaseNumber &&

                        c.Region == model.Region));
            }

            if (model.Month.HasValue)
            {
                feedbackQuery = feedbackQuery.Where(x =>

                    _context.ClosedCaseUploadDetails.Any(c =>

                        c.CaseNumber == x.CaseNumber &&

                        c.ExcelClosedDate.HasValue &&

                        c.ExcelClosedDate.Value.Month == model.Month.Value));
            }

            if (model.Year.HasValue)
            {
                feedbackQuery = feedbackQuery.Where(x =>

                    _context.ClosedCaseUploadDetails.Any(c =>

                        c.CaseNumber == x.CaseNumber &&

                        c.ExcelClosedDate.HasValue &&

                        c.ExcelClosedDate.Value.Year == model.Year.Value));
            }

            if (!string.IsNullOrWhiteSpace(model.FeedbackSource))
            {
                feedbackQuery = feedbackQuery.Where(x =>
                    x.FeedbackSource == model.FeedbackSource);
            }

            if (model.Rating.HasValue)
            {
                feedbackQuery = feedbackQuery.Where(x =>

                    x.Ratings.Any(r =>

                        r.RatingValue.HasValue &&

                        r.RatingValue.Value == model.Rating.Value));
            }

            //===========================================
            // Summary Cards
            //===========================================

            model.TotalCriticalFeedback = await feedbackQuery.CountAsync();

            model.OnlineFeedback = await feedbackQuery
                .CountAsync(x => x.FeedbackSource == "Online");

            model.UploadedFeedback = await feedbackQuery
                .CountAsync(x => x.FeedbackSource == "Upload");

            var ratings = await feedbackQuery
                .SelectMany(x => x.Ratings)
                .Where(r => r.RatingValue.HasValue)
                .Select(r => (decimal?)r.RatingValue)
                .ToListAsync();

            model.AverageRating = ratings.Any()
                ? ratings.Average() ?? 0
                : 0;

            model.TotalRecords = model.TotalCriticalFeedback;

            //===========================================
            // Sorting
            //===========================================

            switch (model.SortColumn)
            {
                case "Company":

                    feedbackQuery = model.SortDirection == "asc"
                        ? feedbackQuery.OrderBy(x => x.CompanyName)
                        : feedbackQuery.OrderByDescending(x => x.CompanyName);

                    break;

                case "Rating":

                    feedbackQuery = model.SortDirection == "asc"
                        ? feedbackQuery.OrderBy(x =>
                            x.Ratings
                             .Where(r => r.RatingValue.HasValue)
                             .Average(r => r.RatingValue.Value))
                        : feedbackQuery.OrderByDescending(x =>
                            x.Ratings
                             .Where(r => r.RatingValue.HasValue)
                             .Average(r => r.RatingValue.Value));

                    break;

                case "ExcelClosedDate":

                    feedbackQuery = model.SortDirection == "asc"
                        ? feedbackQuery.OrderBy(x =>
                            _context.ClosedCaseUploadDetails
                                .Where(c => c.CaseNumber == x.CaseNumber)
                                .Select(c => c.ExcelClosedDate)
                                .FirstOrDefault())
                        : feedbackQuery.OrderByDescending(x =>
                            _context.ClosedCaseUploadDetails
                                .Where(c => c.CaseNumber == x.CaseNumber)
                                .Select(c => c.ExcelClosedDate)
                                .FirstOrDefault());

                    break;

                default:

                    feedbackQuery = model.SortDirection == "asc"
                        ? feedbackQuery.OrderBy(x => x.CreatedDate)
                        : feedbackQuery.OrderByDescending(x => x.CreatedDate);

                    break;
            }

            //===========================================
            // Pagination
            //===========================================

            model.TotalPages = (int)Math.Ceiling(
                (double)model.TotalRecords / model.PageSize);


            //===========================================
            // Feedback List
            //===========================================

            model.Feedbacks = await feedbackQuery

                .Skip((model.PageNumber - 1) * model.PageSize)

                .Take(model.PageSize)

                .Select(x => new CriticalFeedbackListViewModel
                {
                    FeedbackID = x.FeedbackID,

                    CompanyName = x.CompanyName,

                    RespondentName = x.RespondentName,

                    YILEngineer = x.YILEngineer,

                    ServiceRequestNo = x.ServiceRequestNo,

                    FeedbackSource = x.FeedbackSource,

                    CreatedDate = x.CreatedDate,

                    Region = x.Region,

                    ExcelClosedDate = _context.ClosedCaseUploadDetails
                        .Where(c => c.CaseNumber == x.CaseNumber)
                        .OrderByDescending(c => c.UploadID)
                        .Select(c => c.ExcelClosedDate)
                        .FirstOrDefault(),

                    AverageRating = 0
                })

                .ToListAsync();

            foreach (var item in model.Feedbacks)
            {
                var feedbackRatings = await _context.CustomerFeedbackRatings

                    .Where(r => r.FeedbackID == item.FeedbackID)

                    .Where(r => r.RatingValue.HasValue)

                    .Select(r => r.RatingValue.Value)

                    .ToListAsync();

                item.AverageRating = feedbackRatings.Any()
                    ? Convert.ToDecimal(feedbackRatings.Average())
                    : 0;
            }

            return model;


        }
    }
}