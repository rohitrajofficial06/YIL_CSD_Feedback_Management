using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Data;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations
{
    public class RegionDashboardRepository : IRegionDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public RegionDashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RegionDashboardViewModel> GetDashboardAsync()
        {
            RegionDashboardViewModel model = new();

            //------------------------------------------
            // Get Region Wise Count (Single Query)
            //------------------------------------------

            var regionCounts = await _context.ClosedCases

                .GroupBy(x => x.Region)

                .Select(g => new
                {
                    Region = g.Key,

                    Total = g.Count()
                })

                .ToListAsync();

            //------------------------------------------
            // Map Result
            //------------------------------------------

            foreach (var item in regionCounts)
            {
                switch (item.Region)
                {
                    case "East":
                        model.East = item.Total;
                        break;

                    case "West":
                        model.West = item.Total;
                        break;

                    case "North":
                        model.North = item.Total;
                        break;

                    case "South":
                        model.South = item.Total;
                        break;

                    case "Gujarat":
                        model.Gujarat = item.Total;
                        break;

                    case "BHQ":
                        model.BHQ = item.Total;
                        break;
                }
            }

            model.Total =
                model.East +
                model.West +
                model.North +
                model.South +
                model.Gujarat +
                model.BHQ;

            //----------------------------------
            // Today Closed
            //----------------------------------

            DateTime today = DateTime.Today;

            model.TodayClosed =
                await _context.ClosedCases

                .CountAsync(x => x.ClosedDate.Date == today);


            //----------------------------------
            // This Month
            //----------------------------------

            model.ThisMonthClosed =
                await _context.ClosedCases

                .CountAsync(x =>

                    x.ClosedDate.Month == today.Month &&

                    x.ClosedDate.Year == today.Year);


            //----------------------------------
            // Last Upload
            //----------------------------------

            var upload = await _context.ClosedCaseUploads

                .OrderByDescending(x => x.UploadDate)

                .FirstOrDefaultAsync();

            if (upload != null)
            {
                model.LastUploadedFile = upload.FileName;

                model.LastUploadDate = upload.UploadDate;
            }

            //-----------------------------------------
            // Monthly Closure Summary
            //-----------------------------------------

            var monthlyData = await _context.ClosedCases

                .GroupBy(x => new
                {
                    x.ClosedDate.Year,
                    x.ClosedDate.Month
                })

                .Select(g => new MonthlyClosureViewModel
                {
                    Year = g.Key.Year,

                    Month = g.Key.Month,

                    MonthName = new DateTime(
                        g.Key.Year,
                        g.Key.Month,
                        1).ToString("MMM"),

                    TotalCases = g.Count()
                })

                .OrderBy(x => x.Year)

                .ThenBy(x => x.Month)

                .ToListAsync();

            model.MonthlySummary = monthlyData;

            return model;
        }

        public async Task<ClosedCasesViewModel> GetRegionCasesAsync(string region)
        {
            ClosedCasesViewModel model = new();

            model.Cases =
                await (
                    from cc in _context.ClosedCases
                    join fb in _context.CustomerFeedbacks
                        on cc.FeedbackID equals fb.FeedbackID
                    where cc.Region == region
                    orderby cc.ClosedDate descending
                    select new ClosedCaseListViewModel
                    {
                        ClosedCaseID = cc.ClosedCaseID,

                        CaseNumber = cc.CaseNumber,

                        Region = cc.Region,

                        CompanyName = fb.CompanyName,

                        RespondentName = fb.RespondentName,

                        EngineerName = fb.YILEngineer ?? "",

                        ClosedDate = cc.ClosedDate,

                        ClosedBy = cc.ClosedBy ?? ""
                    })
                .ToListAsync();

            model.TotalClosedCases = model.Cases.Count;

            return model;
        }
    }
}