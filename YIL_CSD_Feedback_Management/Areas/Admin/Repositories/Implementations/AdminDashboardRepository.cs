using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Data;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardAsync(int departmentId)
        {
            DashboardViewModel model = new DashboardViewModel();

            // Total Feedback
            model.TotalFeedback = await _context.CustomerFeedbacks
                .Where(x => x.DepartmentID == departmentId)
                .CountAsync();

            // Today's Feedback
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            model.TodayFeedback = await _context.CustomerFeedbacks
                .Where(x => x.DepartmentID == departmentId
                         && x.CreatedDate >= today
                         && x.CreatedDate < tomorrow)
                .CountAsync();

            // Average Rating
            model.AverageRating = await _context.CustomerFeedbackRatings
                .Where(x => x.Feedback != null &&
                            x.Feedback.DepartmentID == departmentId &&
                            x.RatingValue != null)
                .AverageAsync(x => (decimal?)x.RatingValue) ?? 0;

            // Online Feedback
            model.OnlineFeedback = await _context.CustomerFeedbacks
                .Where(x => x.DepartmentID == departmentId &&
                            x.FeedbackSource == "Online")
                .CountAsync();

            // Uploaded Feedback
            model.UploadedFeedback = await _context.CustomerFeedbacks
                .Where(x => x.DepartmentID == departmentId &&
                            x.FeedbackSource == "Upload")
                .CountAsync();

            // Total Questions
            model.TotalQuestions = await _context.FeedbackQuestions
                .Where(x => x.DepartmentID == departmentId)
                .CountAsync();

            // Customer Satisfaction
            model.CustomerSatisfaction = model.AverageRating * 20;

            // Recent Feedback
            model.RecentFeedbacks = await _context.CustomerFeedbacks
                .Where(x => x.DepartmentID == departmentId)
                .OrderByDescending(x => x.CreatedDate)
                .Take(10)
                .Select(x => new RecentFeedbackViewModel
                {
                    FeedbackID = x.FeedbackID,

                    CaseNumber = x.CaseNumber,

                    CompanyName = x.CompanyName,

                    RespondentName = x.RespondentName,

                    EngineerName = x.YILEngineer ?? string.Empty,

                    FeedbackStatus = string.IsNullOrWhiteSpace(x.FeedbackStatus)
                                        ? "Open"
                                        : x.FeedbackStatus,

                    FeedbackSource = x.FeedbackSource,

                    Rating = 0, // We'll calculate this later

                    CreatedDate = x.CreatedDate
                })
                .ToListAsync();
            foreach (var item in model.RecentFeedbacks)
            {
                var feedbackRatings = await _context.CustomerFeedbackRatings
                    .Where(r => r.FeedbackID == item.FeedbackID && r.RatingValue.HasValue)
                    .Select(r => r.RatingValue!.Value)
                    .ToListAsync();

                item.Rating = feedbackRatings.Any()
                    ? (decimal)feedbackRatings.Average()
                    : 0;
            }

            return model;
        }
    }
}