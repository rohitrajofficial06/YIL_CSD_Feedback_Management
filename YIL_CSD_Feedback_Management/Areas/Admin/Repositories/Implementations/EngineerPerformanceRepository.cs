using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Data;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations
{
    public class EngineerPerformanceRepository : IEngineerPerformanceRepository
    {
        private readonly ApplicationDbContext _context;

        public EngineerPerformanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EngineerPerformanceViewModel> GetDashboardAsync()
        {
            EngineerPerformanceViewModel model = new();

            var data =
                await (
                    from cc in _context.ClosedCases

                    join fb in _context.CustomerFeedbacks

                        on cc.FeedbackID equals fb.FeedbackID

                    group new { cc, fb }

                    by new
                    {
                        fb.YILEngineer,
                        cc.Region
                    }

                    into g

                    select new EngineerPerformanceRowViewModel
                    {
                        EngineerName = g.Key.YILEngineer ?? "Not Assigned",

                        Region = g.Key.Region ?? "",

                        ClosedCases = g.Count(),

                        LastClosedDate =
                            g.Max(x => x.cc.ClosedDate)
                    })

                .OrderByDescending(x => x.ClosedCases)

                .ToListAsync();

            model.Engineers = data;

            model.TotalEngineers = data.Count;

            model.TotalClosedCases =
                data.Sum(x => x.ClosedCases);

            if (data.Any())
            {
                model.TopEngineer = data.First().EngineerName;

                model.TopEngineerCases =
                    data.First().ClosedCases;
            }

            return model;
        }

   public async Task<EngineerDetailsViewModel> GetEngineerDetailsAsync(
    EngineerDetailsViewModel filter)
        {
            EngineerDetailsViewModel model = new();

            model.EngineerName = filter.EngineerName;

            //--------------------------------------------
            // Base Query
            //--------------------------------------------

            var query =
                from cc in _context.ClosedCases
                join fb in _context.CustomerFeedbacks
                    on cc.FeedbackID equals fb.FeedbackID
                where fb.YILEngineer == filter.EngineerName
                select new { cc, fb };

            //--------------------------------------------
            // Case Number Filter
            //--------------------------------------------

            if (!string.IsNullOrWhiteSpace(filter.SearchCaseNumber))
            {
                query = query.Where(x =>
                    x.cc.CaseNumber.Contains(filter.SearchCaseNumber));
            }

            //--------------------------------------------
            // Company Filter
            //--------------------------------------------

            if (!string.IsNullOrWhiteSpace(filter.SearchCompany))
            {
                query = query.Where(x =>
                    x.fb.CompanyName.Contains(filter.SearchCompany));
            }

            //--------------------------------------------
            // Region Filter
            //--------------------------------------------

            if (!string.IsNullOrWhiteSpace(filter.Region))
            {
                query = query.Where(x =>
                    x.cc.Region == filter.Region);
            }

            //--------------------------------------------
            // From Date
            //--------------------------------------------

            if (filter.FromDate.HasValue)
            {
                query = query.Where(x =>
                    x.cc.ClosedDate >= filter.FromDate.Value);
            }

            //--------------------------------------------
            // To Date
            //--------------------------------------------

            if (filter.ToDate.HasValue)
            {
                query = query.Where(x =>
                    x.cc.ClosedDate <= filter.ToDate.Value);
            }

            //--------------------------------------------
            // Load Data
            //--------------------------------------------

            model.Cases = await query

                .OrderByDescending(x => x.cc.ClosedDate)

                .Select(x => new EngineerCaseViewModel
                {
                    ClosedCaseID = x.cc.ClosedCaseID,

                    CaseNumber = x.cc.CaseNumber,

                    CompanyName = x.fb.CompanyName,

                    RespondentName = x.fb.RespondentName,

                    Region = x.cc.Region,

                    ClosedDate = x.cc.ClosedDate,

                    ClosedBy = x.cc.ClosedBy ?? ""
                })

                .ToListAsync();

            //--------------------------------------------
            // Summary
            //--------------------------------------------

            model.TotalCases = model.Cases.Count;

            if (model.Cases.Any())
            {
                model.FirstClosedDate =
                    model.Cases.Min(x => x.ClosedDate);

                model.LastClosedDate =
                    model.Cases.Max(x => x.ClosedDate);
            }

            //--------------------------------------------
            // Keep Filter Values
            //--------------------------------------------

            model.SearchCaseNumber = filter.SearchCaseNumber;

            model.SearchCompany = filter.SearchCompany;

            model.Region = filter.Region;

            model.FromDate = filter.FromDate;

            model.ToDate = filter.ToDate;

            return model;
        }
    }
}