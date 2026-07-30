using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly ApplicationDbContext _context;

        public FeedbackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FeedbackIndexViewModel> GetFeedbackAsync(FeedbackIndexViewModel model)
        {
            //===========================================
            // Base Query
            //===========================================

            IQueryable<CustomerFeedback> feedbackQuery = _context.CustomerFeedbacks
                .Include(x => x.Ratings)
                .AsNoTracking();

            //===========================================
            // Department Filter
            //===========================================

            if (model.DepartmentId.HasValue)
            {
                feedbackQuery = feedbackQuery.Where(x =>
                    x.DepartmentID == model.DepartmentId.Value);
            }

            //===========================================
            // Search
            //===========================================

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

            //===========================================
            // Feedback Source
            //===========================================

            if (!string.IsNullOrWhiteSpace(model.FeedbackSource))
            {
                feedbackQuery = feedbackQuery.Where(x =>
                    x.FeedbackSource == model.FeedbackSource);
            }

            if (model.FromMonth.HasValue)
            {
                feedbackQuery = feedbackQuery.Where(x =>
                    _context.ClosedCaseUploadDetails
                        .Any(c =>
                            c.CaseNumber == x.CaseNumber &&
                            c.ExcelClosedDate.HasValue &&
                            c.ExcelClosedDate.Value.Month >= model.FromMonth.Value));
            }

            if (model.ToMonth.HasValue)
            {
                feedbackQuery = feedbackQuery.Where(x =>
                    _context.ClosedCaseUploadDetails
                        .Any(c =>
                            c.CaseNumber == x.CaseNumber &&
                            c.ExcelClosedDate.HasValue &&
                            c.ExcelClosedDate.Value.Month <= model.ToMonth.Value));
            }

            //===========================================
            // Rating Filter
            //===========================================

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

            model.TotalFeedback = await feedbackQuery.CountAsync();

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

            model.TotalRecords = await feedbackQuery.CountAsync();


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

                default:

                    feedbackQuery = model.SortDirection == "asc"
                        ? feedbackQuery.OrderBy(x => x.CreatedDate)
                        : feedbackQuery.OrderByDescending(x => x.CreatedDate);

                    break;
            }


            //===========================================
            // Feedback List
            //===========================================

            model.Feedbacks = await feedbackQuery

      .Skip((model.PageNumber - 1) * model.PageSize)

      .Take(model.PageSize)

      .Select(x => new FeedbackListViewModel
      {
          FeedbackID = x.FeedbackID,
          CompanyName = x.CompanyName,
          RespondentName = x.RespondentName,
          YILEngineer = x.YILEngineer,
          ServiceRequestNo = x.ServiceRequestNo,
          InstrumentCategory = x.InstrumentCategory,
          FeedbackSource = x.FeedbackSource,
          CreatedDate = x.CreatedDate,
          IsSubmitted = x.IsSubmitted,

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
                    .Where(r => r.FeedbackID == item.FeedbackID && r.RatingValue.HasValue)
                    .Select(r => r.RatingValue.Value)
                    .ToListAsync();

                item.AverageRating = feedbackRatings.Any()
                    ? Convert.ToDecimal(feedbackRatings.Average())
                    : 0m;
            }

            return model;
        }

        public async Task<FeedbackDetailsViewModel?> GetDetailsAsync(long feedbackId, int departmentId)
        {
            var feedback = await _context.CustomerFeedbacks
                .Include(x => x.Department)
                .Include(x => x.Ratings)
                .FirstOrDefaultAsync(x =>
                    x.FeedbackID == feedbackId &&
                    x.DepartmentID == departmentId);

            if (feedback == null)
                return null;


            FeedbackDetailsViewModel model = new FeedbackDetailsViewModel
            {
                FeedbackID = feedback.FeedbackID,

                CompanyName = feedback.CompanyName,

                RespondentName = feedback.RespondentName,

                Designation = feedback.Designation,

                ContactNo = feedback.ContactNo,

                EmailID = feedback.EmailID,

                ServiceRequestNo = feedback.ServiceRequestNo,

                InstrumentCategory = feedback.InstrumentCategory,

                YILEngineer = feedback.YILEngineer,

                DepartmentName = feedback.Department?.DepartmentName ?? "",

                Comments = feedback.Comments,

                FeedbackSource = feedback.FeedbackSource,

                CreatedDate = feedback.CreatedDate
            };

            model.Questions = feedback.Ratings
                .OrderBy(x => x.QuestionNo)
                .Select(x => new FeedbackQuestionDetailsViewModel
                {
                    QuestionNo = x.QuestionNo,

                    QuestionText = x.QuestionText,

                    Rating = x.RatingValue,

                    IsNotApplicable = x.IsNotApplicable
                })
                .ToList();

            return model;
        }

    }
}
