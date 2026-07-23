using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Data;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations
{
    public class ClosedCasesRepository : IClosedCasesRepository
    {
        private readonly ApplicationDbContext _context;

        public ClosedCasesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClosedCasesViewModel> GetPageAsync(
     ClosedCasesViewModel filter)
        {
            ClosedCasesViewModel model = new();

            //------------------------------------
            // Total Closed Cases
            //------------------------------------

            model.TotalClosedCases =
                await _context.ClosedCases.CountAsync();

            //------------------------------------
            // Today's Closed Cases
            //------------------------------------

            DateTime today = DateTime.Today;

            model.TodayClosedCases =
                await _context.ClosedCases
                    .CountAsync(x => x.ClosedDate.Date == today);

            //------------------------------------
            // This Month Closed Cases
            //------------------------------------

            model.ThisMonthClosedCases =
                await _context.ClosedCases
                    .CountAsync(x =>
                        x.ClosedDate.Month == DateTime.Today.Month &&
                        x.ClosedDate.Year == DateTime.Today.Year);



            var query =
    from cc in _context.ClosedCases
    join fb in _context.CustomerFeedbacks
        on cc.FeedbackID equals fb.FeedbackID
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
    };

            if (!string.IsNullOrWhiteSpace(filter.SearchCaseNumber))
            {
                query = query.Where(x =>
                    x.CaseNumber.Contains(filter.SearchCaseNumber));
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchEngineer))
            {
                query = query.Where(x =>
                    x.EngineerName.Contains(filter.SearchEngineer));
            }
            if (!string.IsNullOrWhiteSpace(filter.Region))
            {
                query = query.Where(x =>
                    x.Region == filter.Region);
            }
            if (filter.FromDate.HasValue)
            {
                query = query.Where(x =>
                    x.ClosedDate.Date >= filter.FromDate.Value.Date);
            }
            if (filter.ToDate.HasValue)
            {
                query = query.Where(x =>
                    x.ClosedDate.Date <= filter.ToDate.Value.Date);
            }
            //------------------------------------
            // Closed Case List
            //------------------------------------

            model.Cases = await query

     .OrderByDescending(x => x.ClosedDate)

     .ToListAsync();

            model.SearchCaseNumber = filter.SearchCaseNumber;

            model.SearchEngineer = filter.SearchEngineer;

            model.Region = filter.Region;

            model.FromDate = filter.FromDate;

            model.ToDate = filter.ToDate;

            return model;
        }
    }
}