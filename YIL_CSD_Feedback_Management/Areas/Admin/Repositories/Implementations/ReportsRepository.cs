using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Data;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReportsDashboardViewModel> GetDashboardAsync()
        {
            ReportsDashboardViewModel model = new();

            DateTime today = DateTime.Today;

            model.TotalUploads =
                await _context.ClosedCaseUploads.CountAsync();

            model.TotalClosedCases =
                await _context.ClosedCases.CountAsync();

            model.PendingFeedbacks =
                await _context.CustomerFeedbacks
                    .CountAsync(x => x.FeedbackStatus != "Closed");

            model.TotalRegions =
                await _context.ClosedCases
                    .Select(x => x.Region)
                    .Distinct()
                    .CountAsync();

            model.TotalEngineers =
                await _context.CustomerFeedbacks
                    .Select(x => x.YILEngineer)
                    .Distinct()
                    .CountAsync();

            model.TodayClosedCases =
                await _context.ClosedCases
                    .CountAsync(x => x.ClosedDate.Date == today);

            model.ThisMonthClosedCases =
                await _context.ClosedCases
                    .CountAsync(x =>
                        x.ClosedDate.Month == today.Month &&
                        x.ClosedDate.Year == today.Year);

            return model;
        }
    }
}