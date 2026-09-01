using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Helpers;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations
{
    public class ServiceFeedbackAnalyticsRepository
        : IServiceFeedbackAnalyticsRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceFeedbackAnalyticsRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // MAIN ANALYTICS
        // ============================================================

        // ============================================================
        // MAIN ANALYTICS
        // ============================================================

        public async Task<ServiceFeedbackAnalyticsViewModel> GetAnalyticsAsync(
            int month,
            int year)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1);

            // ========================================================
            // REGION LIST
            // Keep BHQ even if there are currently no cases.
            // ========================================================

            var regions = new List<string>
    {
        "East",
        "Gujarat",
        "North",
        "South",
        "West",
        "BHQ"
    };

            // ========================================================
            // CLOSED CASES
            //
            // Source:
            // dbo.tblClosedCaseUploadDetail
            //
            // Date:
            // ExcelClosedDate
            //
            // IMPORTANT:
            // Duplicate CaseNumber must NOT be counted.
            // ========================================================

            var closedCasesRaw =
                await _context.ClosedCaseUploadDetails
                    .Where(x =>
                        x.ExcelClosedDate.HasValue &&
                        x.ExcelClosedDate.Value >= startDate &&
                        x.ExcelClosedDate.Value < endDate &&
                        !string.IsNullOrWhiteSpace(x.CaseNumber))
                    .Select(x => new
                    {
                        x.CaseNumber,
                        x.Region,
                        ClosedDate = x.ExcelClosedDate.Value
                    })
                    .ToListAsync();

            // ========================================================
            // REMOVE DUPLICATE CLOSED CASE NUMBERS
            //
            // One CaseNumber = one closed case
            // ========================================================

            var closedCases = closedCasesRaw
                .GroupBy(x =>
                    x.CaseNumber.Trim().ToUpper())
                .Select(g => g.First())
                .ToList();

            // ========================================================
            // FEEDBACK
            //
            // Source:
            // dbo.trnCustomerFeedback
            //
            // Only submitted feedback is considered.
            //
            // Feedback is filtered by CreatedDate for the
            // selected month.
            // ========================================================

            var feedbackRaw =
                await _context.CustomerFeedbacks
                    .Where(x =>
                        x.CreatedDate >= startDate &&
                        x.CreatedDate < endDate &&
                        x.IsSubmitted &&
                        !string.IsNullOrWhiteSpace(x.CaseNumber))
                    .Select(x => new
                    {
                        x.FeedbackID,
                        x.CaseNumber,
                        x.Region,
                        x.CreatedDate
                    })
                    .ToListAsync();

            // ========================================================
            // REMOVE DUPLICATE FEEDBACK CASE NUMBERS
            //
            // One CaseNumber = one feedback
            // ========================================================

            var feedbacks = feedbackRaw
                .GroupBy(x =>
                    x.CaseNumber!.Trim().ToUpper())
                .Select(g => g.First())
                .ToList();

            // ========================================================
            // FEEDBACK CASE NUMBER LOOKUP
            //
            // Used to match closed cases with feedback.
            // ========================================================

            var feedbackCaseNumbers = feedbacks
                .Select(x =>
                    x.CaseNumber!.Trim().ToUpper())
                .ToHashSet();

            // ========================================================
            // REGION ANALYTICS
            // ========================================================

            var regionAnalytics =
                new List<RegionAnalyticsViewModel>();

            foreach (var region in regions)
            {
                // ----------------------------------------------------
                // Closed cases for selected region
                // ----------------------------------------------------

                var regionClosedCases = closedCases
                    .Where(x =>
                        string.Equals(
                            x.Region?.Trim(),
                            region,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // ----------------------------------------------------
                // Unique CaseNumbers for region
                // ----------------------------------------------------

                var regionClosedCaseNumbers =
                    regionClosedCases
                        .Select(x =>
                            x.CaseNumber.Trim().ToUpper())
                        .ToHashSet();

                int closedCount =
                    regionClosedCaseNumbers.Count;

                // ----------------------------------------------------
                // Feedback Received
                //
                // Match using CaseNumber.
                // Do NOT depend on FeedbackID in
                // tblClosedCaseUploadDetail.
                // ----------------------------------------------------

                int feedbackReceivedCount =
                    regionClosedCaseNumbers
                        .Count(caseNumber =>
                            feedbackCaseNumbers.Contains(caseNumber));

                // ----------------------------------------------------
                // Pending Feedback
                // ----------------------------------------------------

                int pendingCount =
                    Math.Max(
                        closedCount -
                        feedbackReceivedCount,
                        0);

                // ----------------------------------------------------
                // Percentage
                // ----------------------------------------------------

                decimal percentage = 0m;

                if (closedCount > 0)
                {
                    percentage = Math.Round(
                        ((decimal)feedbackReceivedCount /
                         closedCount) * 100m,
                        2,
                        MidpointRounding.AwayFromZero);
                }

                // ----------------------------------------------------
                // Feedback IDs for this region
                //
                // Only feedback belonging to closed cases in this
                // region will be considered for rating.
                // ----------------------------------------------------

                var regionFeedbackIds = feedbacks
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.CaseNumber) &&
                        regionClosedCaseNumbers.Contains(
                            x.CaseNumber.Trim().ToUpper()))
                    .Select(x => x.FeedbackID)
                    .Distinct()
                    .ToList();

                // ----------------------------------------------------
                // Average Rating
                // ----------------------------------------------------

                decimal averageRating = 0m;

                if (regionFeedbackIds.Any())
                {
                    var ratings =
                        await _context.CustomerFeedbackRatings
                            .Where(x =>
                                regionFeedbackIds.Contains(x.FeedbackID) &&
                                x.RatingValue.HasValue &&
                                !x.IsNotApplicable)
                            .Select(x => x.RatingValue!.Value)
                            .ToListAsync();

                    if (ratings.Any())
                    {
                        averageRating = Math.Round(
                            (decimal)ratings.Average(),
                            2,
                            MidpointRounding.AwayFromZero);
                    }
                }

                // ----------------------------------------------------
                // Add Region Result
                // ----------------------------------------------------

                regionAnalytics.Add(
                    new RegionAnalyticsViewModel
                    {
                        Region = region,

                        ClosedCases =
                            closedCount,

                        FeedbackReceived =
                            feedbackReceivedCount,

                        PendingFeedback =
                            pendingCount,

                        Percentage =
                            percentage,

                        AverageRating =
                            averageRating
                    });
            }

            // ========================================================
            // TOTAL CLOSED CASES
            // ========================================================

            int totalClosedCases =
                closedCases
                    .Select(x =>
                        x.CaseNumber.Trim().ToUpper())
                    .Distinct()
                    .Count();

            // ========================================================
            // TOTAL FEEDBACK RECEIVED
            //
            // Only feedback whose CaseNumber exists in the
            // selected month's closed cases is counted.
            // ========================================================

            var allClosedCaseNumbers =
                closedCases
                    .Select(x =>
                        x.CaseNumber.Trim().ToUpper())
                    .ToHashSet();

            int totalFeedbackReceived =
                feedbackCaseNumbers
                    .Count(caseNumber =>
                        allClosedCaseNumbers.Contains(caseNumber));

            // ========================================================
            // TOTAL PENDING FEEDBACK
            // ========================================================

            int totalPendingFeedback =
                Math.Max(
                    totalClosedCases -
                    totalFeedbackReceived,
                    0);

            // ========================================================
            // TOTAL PERCENTAGE
            // ========================================================

            decimal totalPercentage = 0m;

            if (totalClosedCases > 0)
            {
                totalPercentage = Math.Round(
                    ((decimal)totalFeedbackReceived /
                     totalClosedCases) * 100m,
                    2,
                    MidpointRounding.AwayFromZero);
            }

            // ========================================================
            // OVERALL AVERAGE RATING
            // ========================================================

            var allFeedbackIds = feedbacks
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.CaseNumber) &&
                    allClosedCaseNumbers.Contains(
                        x.CaseNumber.Trim().ToUpper()))
                .Select(x => x.FeedbackID)
                .Distinct()
                .ToList();

            decimal overallAverageRating = 0m;

            if (allFeedbackIds.Any())
            {
                var allRatings =
                    await _context.CustomerFeedbackRatings
                        .Where(x =>
                            allFeedbackIds.Contains(x.FeedbackID) &&
                            x.RatingValue.HasValue &&
                            !x.IsNotApplicable)
                        .Select(x => x.RatingValue!.Value)
                        .ToListAsync();

                if (allRatings.Any())
                {
                    overallAverageRating = Math.Round(
                        (decimal)allRatings.Average(),
                        2,
                        MidpointRounding.AwayFromZero);
                }
            }

            // ========================================================
            // MONTHLY TREND
            // ========================================================

            var monthlyTrend =
                await GetMonthlyTrendAsync(
                    month,
                    year);

            // ========================================================
            // FINAL VIEW MODEL
            // ========================================================

            return new ServiceFeedbackAnalyticsViewModel
            {
                Month = month,

                Year = year,

                TotalClosedCases =
                    totalClosedCases,

                TotalFeedbackReceived =
                    totalFeedbackReceived,

                TotalPendingFeedback =
                    totalPendingFeedback,

                CompletionPercentage =
                    totalPercentage,

                AverageRating =
                    overallAverageRating,

                Regions =
                    regionAnalytics,

                MonthlyTrend =
                    monthlyTrend
            };
        }

        // ============================================================
        // MONTHLY TREND
        // ============================================================

        private async Task<List<MonthlyTrendViewModel>>
            GetMonthlyTrendAsync(
                int selectedMonth,
                int selectedYear)
        {
            var result =
                new List<MonthlyTrendViewModel>();

            // ========================================================
            // LAST 12 MONTHS
            // Includes selected month
            // ========================================================

            DateTime selectedDate =
                new DateTime(
                    selectedYear,
                    selectedMonth,
                    1);

            DateTime firstMonth =
                selectedDate.AddMonths(-11);

            DateTime endMonth =
                selectedDate.AddMonths(1);

            // ========================================================
            // CLOSED CASES
            //
            // Source:
            // tblClosedCaseUploadDetail
            //
            // Date:
            // ExcelClosedDate
            //
            // Duplicate CaseNumber will be removed.
            // ========================================================

            var closedCasesRaw =
                await _context.ClosedCaseUploadDetails
                    .Where(x =>
                        x.ExcelClosedDate.HasValue &&
                        x.ExcelClosedDate.Value >= firstMonth &&
                        x.ExcelClosedDate.Value < endMonth &&
                        !string.IsNullOrWhiteSpace(x.CaseNumber))
                    .Select(x => new
                    {
                        x.CaseNumber,
                        x.Region,
                        ClosedDate = x.ExcelClosedDate.Value
                    })
                    .ToListAsync();

            // ========================================================
            // REMOVE DUPLICATE CLOSED CASE NUMBERS
            // ========================================================

            var closedCases =
                closedCasesRaw
                    .GroupBy(x =>
                        x.CaseNumber.Trim().ToUpper())
                    .Select(g => g.First())
                    .ToList();

            // ========================================================
            // FEEDBACK
            //
            // Source:
            // trnCustomerFeedback
            //
            // Only submitted feedback is considered.
            // ========================================================

            var feedbackRaw =
                await _context.CustomerFeedbacks
                    .Where(x =>
                        x.CreatedDate >= firstMonth &&
                        x.CreatedDate < endMonth &&
                        x.IsSubmitted &&
                        !string.IsNullOrWhiteSpace(x.CaseNumber))
                    .Select(x => new
                    {
                        x.FeedbackID,
                        x.CaseNumber,
                        x.CreatedDate
                    })
                    .ToListAsync();

            // ========================================================
            // REMOVE DUPLICATE FEEDBACK CASE NUMBERS
            // ========================================================

            var feedbacks =
                feedbackRaw
                    .GroupBy(x =>
                        x.CaseNumber!.Trim().ToUpper())
                    .Select(g => g.First())
                    .ToList();

            // ========================================================
            // BUILD FEEDBACK CASE NUMBER LOOKUP
            // ========================================================

            var feedbackCaseNumbers =
                feedbacks
                    .Select(x =>
                        x.CaseNumber!.Trim().ToUpper())
                    .ToHashSet();

            // ========================================================
            // CREATE 12 MONTHS
            // ========================================================

            for (int i = 0; i < 12; i++)
            {
                DateTime monthStart =
                    firstMonth.AddMonths(i);

                DateTime monthEnd =
                    monthStart.AddMonths(1);

                // ----------------------------------------------------
                // Closed cases for this month
                // ----------------------------------------------------

                var monthClosedCases =
                    closedCases
                        .Where(x =>
                            x.ClosedDate >= monthStart &&
                            x.ClosedDate < monthEnd)
                        .ToList();

                // ----------------------------------------------------
                // Unique closed CaseNumbers
                // ----------------------------------------------------

                var monthClosedCaseNumbers =
                    monthClosedCases
                        .Select(x =>
                            x.CaseNumber.Trim().ToUpper())
                        .ToHashSet();

                int closedCount =
                    monthClosedCaseNumbers.Count;

                // ----------------------------------------------------
                // Feedback received
                //
                // Feedback must:
                // 1. Be created in this month
                // 2. Match a closed CaseNumber for this month
                // ----------------------------------------------------

                var monthFeedbacks =
                    feedbacks
                        .Where(x =>
                            x.CreatedDate >= monthStart &&
                            x.CreatedDate < monthEnd &&
                            monthClosedCaseNumbers.Contains(
                                x.CaseNumber!.Trim().ToUpper()))
                        .ToList();

                int feedbackCount =
                    monthFeedbacks
                        .Select(x =>
                            x.CaseNumber!.Trim().ToUpper())
                        .Distinct()
                        .Count();

                // ----------------------------------------------------
                // Percentage
                // ----------------------------------------------------

                decimal percentage = 0m;

                if (closedCount > 0)
                {
                    percentage =
                        Math.Round(
                            ((decimal)feedbackCount /
                             closedCount) * 100m,
                            2,
                            MidpointRounding.AwayFromZero);
                }

                // ----------------------------------------------------
                // Average Rating
                // ----------------------------------------------------

                decimal averageRating = 0m;

                var monthFeedbackIds =
                    monthFeedbacks
                        .Select(x => x.FeedbackID)
                        .Distinct()
                        .ToList();

                if (monthFeedbackIds.Any())
                {
                    var ratings =
                        await _context.CustomerFeedbackRatings
                            .Where(x =>
                                monthFeedbackIds.Contains(
                                    x.FeedbackID) &&
                                x.RatingValue.HasValue &&
                                !x.IsNotApplicable)
                            .Select(x =>
                                x.RatingValue!.Value)
                            .ToListAsync();

                    if (ratings.Any())
                    {
                        averageRating =
                            Math.Round(
                                (decimal)ratings.Average(),
                                2,
                                MidpointRounding.AwayFromZero);
                    }
                }

                // ----------------------------------------------------
                // Add monthly result
                // ----------------------------------------------------

                result.Add(
                    new MonthlyTrendViewModel
                    {
                        MonthName =
                            monthStart.ToString("MMM-yyyy"),

                        ClosedCases =
                            closedCount,

                        FeedbackReceived =
                            feedbackCount,

                        Percentage =
                            percentage,

                        AverageRating =
                            averageRating
                    });
            }

            return result;
        }

        // ============================================================
        // REGION DETAILS / PENDING FEEDBACK
        // ============================================================

        public async Task<List<RegionDetailsViewModel>>
            GetRegionDetailsAsync(
                string region,
                int? month,
                int? year)
        {
            // ========================================================
            // VALIDATE REGION
            // ========================================================

            if (string.IsNullOrWhiteSpace(region))
            {
                return new List<RegionDetailsViewModel>();
            }

            // ========================================================
            // DEFAULT MONTH / YEAR
            // ========================================================

            DateTime now = DateTime.Now;

            int selectedMonth =
                month ?? now.Month;

            int selectedYear =
                year ?? now.Year;

            DateTime startDate =
                new DateTime(
                    selectedYear,
                    selectedMonth,
                    1);

            DateTime endDate =
                startDate.AddMonths(1);

            // ========================================================
            // CLOSED CASES
            //
            // Source:
            // tblClosedCaseUploadDetail
            //
            // Date:
            // ExcelClosedDate
            // ========================================================

            var closedCasesRaw =
                await _context.ClosedCaseUploadDetails
                    .Where(x =>
                        x.ExcelClosedDate.HasValue &&
                        x.ExcelClosedDate.Value >= startDate &&
                        x.ExcelClosedDate.Value < endDate &&
                        !string.IsNullOrWhiteSpace(x.CaseNumber) &&
                        x.Region != null &&
                        x.Region.Trim().ToUpper()
                            == region.Trim().ToUpper())
                    .Select(x => new
                    {
                        x.CaseNumber,
                        x.Region,
                        ClosedDate = x.ExcelClosedDate.Value
                    })
                    .ToListAsync();

            // ========================================================
            // REMOVE DUPLICATE CASE NUMBERS
            //
            // One CaseNumber = one closed case
            // ========================================================

            var closedCases =
                closedCasesRaw
                    .GroupBy(x =>
                        x.CaseNumber.Trim().ToUpper())
                    .Select(g => g.First())
                    .ToList();

            // ========================================================
            // IF NO CLOSED CASES
            // ========================================================

            if (!closedCases.Any())
            {
                return new List<RegionDetailsViewModel>();
            }

            // ========================================================
            // GET CASE NUMBERS
            // ========================================================

            var closedCaseNumbers =
                closedCases
                    .Select(x =>
                        x.CaseNumber.Trim().ToUpper())
                    .ToHashSet();

            // ========================================================
            // GET SUBMITTED FEEDBACK
            //
            // Source:
            // trnCustomerFeedback
            //
            // Only submitted feedback is considered.
            // ========================================================

            var feedbacksRaw =
                await _context.CustomerFeedbacks
                    .Where(x =>
                        x.IsSubmitted &&
                        !string.IsNullOrWhiteSpace(x.CaseNumber))
                    .Select(x => new
                    {
                        x.FeedbackID,
                        x.CaseNumber,
                        x.YILEngineer
                    })
                    .ToListAsync();

            // ========================================================
            // REMOVE DUPLICATE FEEDBACK CASE NUMBERS
            // ========================================================

            var feedbacks =
                feedbacksRaw
                    .GroupBy(x =>
                        x.CaseNumber!.Trim().ToUpper())
                    .Select(g => g.First())
                    .ToList();

            // ========================================================
            // FIND PENDING CASES
            //
            // Closed CaseNumber that does not exist in submitted
            // feedback.
            // ========================================================

            var pendingCases =
                closedCases
                    .Where(x =>
                        !feedbacks.Any(f =>
                            string.Equals(
                                f.CaseNumber?.Trim(),
                                x.CaseNumber.Trim(),
                                StringComparison.OrdinalIgnoreCase)))
                    .ToList();

            // ========================================================
            // GET ENGINEER INFORMATION
            //
            // Feedback does not exist for pending cases, so there
            // may not be an engineer available from trnCustomerFeedback.
            // Therefore we don't invent engineer information.
            // ========================================================

            var result =
                pendingCases
                    .Select(x => new RegionDetailsViewModel
                    {
                        CaseNumber =
                            x.CaseNumber,

                        YILEngineer =
                            null,

                        ClosedDate =
                            x.ClosedDate,

                        FeedbackReceived =
                            false,

                        Rating =
                            null
                    })
                    .OrderBy(x => x.CaseNumber)
                    .ToList();

            return result;
        }


        // ============================================================
        // EXPORT TO EXCEL
        // ============================================================

        public async Task<FileResult> ExportToExcelAsync(
            int? month,
            int? year)
        {
            int selectedMonth =
                month ?? DateTime.Now.Month;

            int selectedYear =
                year ?? DateTime.Now.Year;

            DateTime startDate =
                new DateTime(
                    selectedYear,
                    selectedMonth,
                    1);

            DateTime endDate =
                startDate.AddMonths(1);

            var closedCases =
                await _context.ClosedCaseUploadDetails
                    .Where(x =>
                        x.ExcelClosedDate.HasValue &&
                        x.ExcelClosedDate.Value >= startDate &&
                        x.ExcelClosedDate.Value < endDate)
                    .OrderBy(x => x.Region)
                    .ThenBy(x => x.CaseNumber)
                    .ToListAsync();

            var feedbackIds =
                closedCases
                    .Where(x => x.FeedbackID.HasValue)
                    .Select(x => x.FeedbackID!.Value)
                    .Distinct()
                    .ToList();

            var feedbackData =
                await _context.CustomerFeedbacks
                    .Where(x =>
                        feedbackIds.Contains(x.FeedbackID))
                    .Select(x => new
                    {
                        x.FeedbackID,
                        x.YILEngineer
                    })
                    .ToListAsync();

            using var workbook =
                new XLWorkbook();

            var worksheet =
                workbook.Worksheets.Add(
                    "Feedback Analytics");

            // --------------------------------------------------------
            // Header
            // --------------------------------------------------------

            worksheet.Cell(1, 1)
                .Value = "Feedback Analytics";

            worksheet.Cell(2, 1)
                .Value = "Month";

            worksheet.Cell(2, 2)
                .Value =
                    startDate.ToString("MMMM yyyy");

            worksheet.Cell(4, 1)
                .Value = "Case Number";

            worksheet.Cell(4, 2)
                .Value = "Region";

            worksheet.Cell(4, 3)
                .Value = "Closed Date";

            worksheet.Cell(4, 4)
                .Value = "Engineer";

            worksheet.Cell(4, 5)
                .Value = "Feedback Status";

            worksheet.Cell(4, 6)
                .Value = "Rating";

            // --------------------------------------------------------
            // Data
            // --------------------------------------------------------

            int row = 5;

            foreach (var item in closedCases)
            {
                var feedback =
                    item.FeedbackID.HasValue
                        ? feedbackData.FirstOrDefault(
                            x => x.FeedbackID ==
                                 item.FeedbackID.Value)
                        : null;

                worksheet.Cell(row, 1)
                    .Value = item.CaseNumber;

                worksheet.Cell(row, 2)
                    .Value = item.Region;

                worksheet.Cell(row, 3)
                    .Value =
                        item.ExcelClosedDate;

                worksheet.Cell(row, 4)
                    .Value =
                        feedback?.YILEngineer ?? "-";

                worksheet.Cell(row, 5)
                    .Value =
                        item.FeedbackID.HasValue
                            ? "Received"
                            : "Pending";

                worksheet.Cell(row, 6)
                    .Value = "-";

                row++;
            }

            // --------------------------------------------------------
            // Formatting
            // --------------------------------------------------------

            var headerRange =
                worksheet.Range(
                    "A4:F4");

            headerRange.Style.Font.Bold =
                true;

            headerRange.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            worksheet.Columns()
                .AdjustToContents();

            worksheet.Column(3)
                .Style.DateFormat.Format =
                "dd-MMM-yyyy";

            // --------------------------------------------------------
            // Save Excel
            // --------------------------------------------------------

            using var stream =
                new MemoryStream();

            workbook.SaveAs(stream);

            stream.Position = 0;

            string fileName =
                $"Feedback_Analytics_{startDate:MMM-yyyy}.xlsx";

            return new FileContentResult(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = fileName
            };
        }


        // ============================================================
        // DECIMAL TRUNCATION
        // ============================================================

        private static decimal TruncateDecimal(
            decimal value,
            int decimals)
        {
            decimal factor =
                (decimal)Math.Pow(
                    10,
                    decimals);

            return Math.Truncate(
                value * factor) / factor;
        }
    }
}