using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.ViewModels.Dashboard;

namespace YIL_CSD_Feedback_Management.Repositories.Implementations
{
    public class EngineerDashboardRepository : IEngineerDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public EngineerDashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EngineerDashboardViewModel> GetDashboardAsync(
      string createdBy,
      string fullName,
      string region,
      string module,
      int departmentId)
        {
            //==========================================================
            // Load Feedbacks with Ratings
            //==========================================================

            var feedbacks = await _context.CustomerFeedbacks
                .Include(x => x.Ratings)
                .Where(x =>
                    x.CreatedBy == createdBy &&
                    x.IsActive &&
                    x.DepartmentID == departmentId)
                .ToListAsync();

            //==========================================================
            // Dashboard Model
            //==========================================================

            EngineerDashboardViewModel model = new();

            model.FullName = fullName;
            model.Region = region;
            model.Module = module;
            model.DepartmentId = departmentId;

            //==========================================================
            // Feedback Count
            //==========================================================

            model.TotalFeedback = feedbacks.Count;

            model.OpenFeedback = feedbacks.Count(x =>
                x.FeedbackStatus == "Open");

            model.ClosedFeedback = feedbacks.Count(x =>
                x.FeedbackStatus == "Closed");

            //==========================================================
            // Greeting
            //==========================================================

            model.Greeting = GetGreeting();

            //==========================================================
            // Average Rating
            //==========================================================

            var ratings = feedbacks
                .SelectMany(x => x.Ratings)
                .Where(r =>
                    !r.IsNotApplicable &&
                    r.RatingValue.HasValue)
                .Select(r => r.RatingValue!.Value)
                .ToList();

            model.AverageRating = ratings.Any()
                ? Math.Round(ratings.Average(), 1)
                : 0;

            //==========================================================
            // Recent Feedback
            //==========================================================

            model.RecentFeedbacks = feedbacks
                .OrderByDescending(x => x.CreatedDate)
                .Take(5)
                .Select(x => new RecentFeedbackViewModel
                {
                    FeedbackID = x.FeedbackID,

                    CaseNumber = x.CaseNumber ?? string.Empty,

                    CompanyName = x.CompanyName,

                    FeedbackStatus = x.FeedbackStatus,

                    CreatedDate = x.CreatedDate
                })
                .ToList();

            return model;
        }
        private string GetGreeting()
        {
            int hour = DateTime.Now.Hour;

            if (hour < 12)
                return "Good Morning";

            if (hour < 17)
                return "Good Afternoon";

            return "Good Evening";
        }
    }
}