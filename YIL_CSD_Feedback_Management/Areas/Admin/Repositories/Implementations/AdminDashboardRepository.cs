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

            //===========================================
            // Recent Feedback
            //===========================================

            model.RecentFeedbacks = await _context.CustomerFeedbacks
                .Where(x => x.DepartmentID == departmentId)
                .OrderByDescending(x => x.CreatedDate)
                .Take(10)
                .Select(x => new RecentFeedbackViewModel
                {
                    FeedbackID = x.FeedbackID,

                    CaseNumber = x.CaseNumber ?? string.Empty,

                    CompanyName = x.CompanyName,

                    RespondentName = x.RespondentName,

                    EngineerName = x.YILEngineer ?? string.Empty,

                    FeedbackStatus = string.IsNullOrWhiteSpace(x.FeedbackStatus)
                                        ? "Open"
                                        : x.FeedbackStatus,

                    FeedbackSource = x.FeedbackSource,

                    // Calculate Average Rating
                    Rating = x.Ratings
                        .Where(r => r.RatingValue.HasValue)
                        .Select(r => (decimal?)r.RatingValue)
                        .Average() ?? 0,

                    CreatedDate = x.CreatedDate,

                    FeedbackFileName = x.FeedbackFileName,

                    FeedbackFilePath = x.FeedbackFilePath,

                    // Excel Closed Date
                    ExcelClosedDate = _context.ClosedCaseUploadDetails
                        .Where(c => c.CaseNumber == x.CaseNumber)
                        .OrderByDescending(c => c.UploadID)
                        .Select(c => c.ExcelClosedDate)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return model;
        }
    }
}