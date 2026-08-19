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


        // ==========================================================
        // REPORTS DASHBOARD
        // ==========================================================

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


        // ==========================================================
        // PENDING FEEDBACK REPORT
        // ==========================================================

        public async Task<PendingFeedbackViewModel>
            GetPendingFeedbackAsync(
                string? searchText,
                string? region,
                DateTime? fromDate,
                DateTime? toDate,
                long? uploadId,
                int pageNumber,
                int pageSize)
        {
            PendingFeedbackViewModel model = new();

            // ------------------------------------------------------
            // Base Query
            //
            // Closed Excel records where Customer Feedback
            // does NOT exist for the Case Number.
            // ------------------------------------------------------

            var query =
                from detail in _context.ClosedCaseUploadDetails

                join upload in _context.ClosedCaseUploads
                    on detail.UploadID equals upload.UploadID

                where !_context.CustomerFeedbacks
                    .Any(f =>
                        f.CaseNumber == detail.CaseNumber)

                select new
                {
                    Detail = detail,
                    Upload = upload
                };


            // ------------------------------------------------------
            // Search
            // ------------------------------------------------------

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();

                query = query.Where(x =>
                    x.Detail.CaseNumber.Contains(searchText) ||
                    x.Detail.CaseOwnerOrg.Contains(searchText));
            }


            // ------------------------------------------------------
            // Region
            // ------------------------------------------------------

            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(x =>
                    x.Detail.Region == region);
            }


            // ------------------------------------------------------
            // Upload Date - From
            // ------------------------------------------------------

            if (fromDate.HasValue)
            {
                DateTime from =
                    fromDate.Value.Date;

                query = query.Where(x =>
                    x.Upload.UploadDate >= from);
            }


            // ------------------------------------------------------
            // Upload Date - To
            // ------------------------------------------------------

            if (toDate.HasValue)
            {
                DateTime to =
                    toDate.Value.Date.AddDays(1);

                query = query.Where(x =>
                    x.Upload.UploadDate < to);
            }


            // ------------------------------------------------------
            // Specific Upload
            // ------------------------------------------------------

            if (uploadId.HasValue)
            {
                query = query.Where(x =>
                    x.Upload.UploadID == uploadId.Value);
            }


            // ------------------------------------------------------
            // Summary
            // ------------------------------------------------------

            model.TotalPending =
                await query.CountAsync();


            model.EastPending =
                await query.CountAsync(x =>
                    x.Detail.Region == "East");

            model.WestPending =
                await query.CountAsync(x =>
                    x.Detail.Region == "West");

            model.NorthPending =
                await query.CountAsync(x =>
                    x.Detail.Region == "North");

            model.SouthPending =
                await query.CountAsync(x =>
                    x.Detail.Region == "South");

            model.GujaratPending =
                await query.CountAsync(x =>
                    x.Detail.Region == "Gujarat");

            model.BHQPending =
                await query.CountAsync(x =>
                    x.Detail.Region == "BHQ");


            // ------------------------------------------------------
            // Total Records
            // ------------------------------------------------------

            model.TotalRecords =
                await query.CountAsync();


            // ------------------------------------------------------
            // Pagination
            // ------------------------------------------------------

            if (pageNumber < 1)
                pageNumber = 1;

            model.PageNumber = pageNumber;

            model.PageSize = pageSize;


            // ------------------------------------------------------
            // Records
            // ------------------------------------------------------

            var records = await query
                .OrderByDescending(x => x.Upload.UploadDate)
                .ThenBy(x => x.Detail.CaseNumber)

                .Skip(
                    (pageNumber - 1) *
                    pageSize)

                .Take(pageSize)

                .Select(x =>
                    new PendingFeedbackRecordViewModel
                    {
                        DetailID =
                            x.Detail.DetailID,

                        CaseNumber =
                            x.Detail.CaseNumber,

                        CaseOwnerOrg =
                            x.Detail.CaseOwnerOrg ?? "",

                        Region =
                            x.Detail.Region ?? "",

                        MatchStatus =
                            "Feedback Pending",

                        Remarks =
                            "Customer feedback not entered.",

                        UploadID =
                            x.Upload.UploadID,

                        FileName =
                            x.Upload.FileName ?? "",

                        UploadedBy =
                            x.Upload.UploadedBy ?? "",

                        UploadDate =
                            x.Upload.UploadDate
                    })

                .ToListAsync();


            // ------------------------------------------------------
            // Serial Number
            // ------------------------------------------------------

            int serialNumber =
                ((pageNumber - 1) * pageSize) + 1;

            foreach (var record in records)
            {
                record.SerialNo =
                    serialNumber++;
            }


            model.Records = records;


            // ------------------------------------------------------
            // Upload Dropdown
            // ------------------------------------------------------

            model.Uploads =
                await _context.ClosedCaseUploads

                    .OrderByDescending(x =>
                        x.UploadDate)

                    .Select(x =>
                        new PendingFeedbackUploadViewModel
                        {
                            UploadID =
                                x.UploadID,

                            FileName =
                                x.FileName ?? "",

                            UploadedBy =
                                x.UploadedBy ?? "",

                            UploadDate =
                                x.UploadDate,

                            TotalCases =
                                x.TotalCases,

                            MatchedCases =
                                x.MatchedCases,

                            UnMatchedCases =
                                x.UnMatchedCases
                        })

                    .ToListAsync();


            return model;
        }
    }
}