using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Data;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations
{
    public class ServiceFeedbackAnalyticsRepository : IServiceFeedbackAnalyticsRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceFeedbackAnalyticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceFeedbackAnalyticsViewModel> GetAnalyticsAsync(
      ServiceFeedbackAnalyticsViewModel model)
        {
            //==========================================
            // Default Month & Year
            //==========================================

            model.Month ??= DateTime.Now.Month;
            model.Year ??= DateTime.Now.Year;

            //==========================================
            // Closed Cases Query
            //==========================================

            var closedCasesQuery = _context.ClosedCaseUploadDetails
                .AsNoTracking()
                .Where(x =>
                    x.ExcelClosedDate.HasValue &&
                    x.ExcelClosedDate.Value.Month == model.Month &&
                    x.ExcelClosedDate.Value.Year == model.Year);


            //==========================================
            // Dashboard Summary
            //==========================================

            model.TotalClosedCases = await closedCasesQuery
    .Select(x => x.CaseNumber)
    .Distinct()
    .CountAsync();

            model.TotalFeedbackReceived = await
                 (
                     from feedback in _context.CustomerFeedbacks.AsNoTracking()

                     join closed in _context.ClosedCaseUploadDetails.AsNoTracking()
                         on feedback.CaseNumber equals closed.CaseNumber

                     where closed.ExcelClosedDate.HasValue
                           && closed.ExcelClosedDate.Value.Month == model.Month
                           && closed.ExcelClosedDate.Value.Year == model.Year

                     select feedback.FeedbackID

                 )
                 .Distinct()
                 .CountAsync();

            model.CompletionPercentage =
                model.TotalClosedCases == 0
                ? 0
                : Math.Round(
                    ((decimal)model.TotalFeedbackReceived /
                     model.TotalClosedCases) * 100, 2);

            // Overall Average Rating (Selected Month Only)
            var averageRating = await
            (
                from rating in _context.CustomerFeedbackRatings.AsNoTracking()

                join feedback in _context.CustomerFeedbacks.AsNoTracking()
                    on rating.FeedbackID equals feedback.FeedbackID

                join closed in _context.ClosedCaseUploadDetails.AsNoTracking()
                    on feedback.CaseNumber equals closed.CaseNumber

                where rating.RatingValue.HasValue
                      && closed.ExcelClosedDate.HasValue
                      && closed.ExcelClosedDate.Value.Month == model.Month
                      && closed.ExcelClosedDate.Value.Year == model.Year

                select (decimal?)rating.RatingValue

            ).AverageAsync();

            model.AverageRating = Math.Round(averageRating ?? 0, 2);

            //==========================================
            // Region Analytics
            //==========================================
            var closedRegionData = await closedCasesQuery
      .GroupBy(x => string.IsNullOrWhiteSpace(x.Region) ? "Unknown" : x.Region)
      .Select(g => new
      {
          Region = g.Key,

          ClosedCases = g
              .Select(x => x.CaseNumber)
              .Distinct()
              .Count()
      })
      .ToListAsync();

            var feedbackRegionData = await
 (
     from feedback in _context.CustomerFeedbacks.AsNoTracking()

     join closed in _context.ClosedCaseUploadDetails.AsNoTracking()
         on feedback.CaseNumber equals closed.CaseNumber

     where closed.ExcelClosedDate.HasValue
           && closed.ExcelClosedDate.Value.Month == model.Month
           && closed.ExcelClosedDate.Value.Year == model.Year

     group feedback by (string.IsNullOrWhiteSpace(closed.Region) ? "Unknown" : closed.Region) into g

     select new
     {
         Region = g.Key,

         FeedbackReceived = g
             .Select(x => x.FeedbackID)
             .Distinct()
             .Count()
     }

 ).ToListAsync();

            var ratingRegionData = await


(
    from rating in _context.CustomerFeedbackRatings.AsNoTracking()

    join feedback in _context.CustomerFeedbacks.AsNoTracking()
        on rating.FeedbackID equals feedback.FeedbackID

    join closed in _context.ClosedCaseUploadDetails.AsNoTracking()
        on feedback.CaseNumber equals closed.CaseNumber

    where rating.RatingValue.HasValue
          && closed.ExcelClosedDate.HasValue
          && closed.ExcelClosedDate.Value.Month == model.Month
          && closed.ExcelClosedDate.Value.Year == model.Year

    group rating by (string.IsNullOrWhiteSpace(closed.Region)
                      ? "Unknown"
                      : closed.Region) into g

    select new
    {
        Region = g.Key,

        AverageRating = g.Average(x => (decimal)x.RatingValue.Value)
    }

).ToListAsync();

            var closedDictionary = closedRegionData
    .ToDictionary(x => x.Region);

            var feedbackDictionary = feedbackRegionData
                .ToDictionary(x => x.Region);

            var ratingDictionary = ratingRegionData
                .ToDictionary(x => x.Region);

            var allRegions = closedRegionData
    .Select(x => x.Region)
    .Union(feedbackRegionData.Select(x => x.Region))
    .Union(ratingRegionData.Select(x => x.Region))
    .Distinct()
    .OrderBy(x => x)
    .ToList();

            foreach (var region in allRegions)
            {
                closedDictionary.TryGetValue(region, out var closed);

                feedbackDictionary.TryGetValue(region, out var feedback);

                ratingDictionary.TryGetValue(region, out var rating);

                int closedCases = closed?.ClosedCases ?? 0;

                int feedbackReceived = feedback?.FeedbackReceived ?? 0;

                decimal completion = closedCases == 0
                    ? 0
                    : Math.Round(
                        ((decimal)feedbackReceived / closedCases) * 100, 2);

                model.Regions.Add(new RegionAnalyticsViewModel
                {
                    Region = region,

                    ClosedCases = closedCases,

                    FeedbackReceived = feedbackReceived,

                    CompletionPercentage = completion,

                    AverageRating = (decimal)(rating?.AverageRating ?? 0),

                    Status = GetStatus(completion)
                });
            }

            var monthlyClosedCases = await _context.ClosedCaseUploadDetails
     .AsNoTracking()
     .Where(x =>
         x.ExcelClosedDate.HasValue &&
         x.ExcelClosedDate.Value.Year == model.Year)
     .GroupBy(x => x.ExcelClosedDate.Value.Month)
     .Select(g => new
     {
         Month = g.Key,

         ClosedCases = g
             .Select(x => x.CaseNumber)
             .Distinct()
             .Count()
     })
     .ToListAsync();

            var monthlyFeedback = await
(
    from feedback in _context.CustomerFeedbacks.AsNoTracking()

    join closed in _context.ClosedCaseUploadDetails.AsNoTracking()
        on feedback.CaseNumber equals closed.CaseNumber

    where closed.ExcelClosedDate.HasValue
          && closed.ExcelClosedDate.Value.Year == model.Year

    group feedback by closed.ExcelClosedDate.Value.Month into g

    select new
    {
        Month = g.Key,

        FeedbackReceived = g
            .Select(x => x.FeedbackID)
            .Distinct()
            .Count()
    }

).ToListAsync();


            var monthlyRatings = await
(
    from rating in _context.CustomerFeedbackRatings.AsNoTracking()

    join feedback in _context.CustomerFeedbacks.AsNoTracking()
        on rating.FeedbackID equals feedback.FeedbackID

    join closed in _context.ClosedCaseUploadDetails.AsNoTracking()
        on feedback.CaseNumber equals closed.CaseNumber

    where rating.RatingValue.HasValue
          && closed.ExcelClosedDate.HasValue
          && closed.ExcelClosedDate.Value.Year == model.Year

    group rating by closed.ExcelClosedDate.Value.Month into g

    select new
    {
        Month = g.Key,

        AverageRating = g.Average(x => (decimal)x.RatingValue.Value)
    }

).ToListAsync();

            //==========================================
            // Monthly Trend
            //==========================================

            var monthlyFeedbackDictionary = monthlyFeedback
                .ToDictionary(x => x.Month);

            var monthlyRatingDictionary = monthlyRatings
                .ToDictionary(x => x.Month);

            model.MonthlyTrend.Clear();

            foreach (var month in monthlyClosedCases.OrderBy(x => x.Month))
            {
                monthlyFeedbackDictionary.TryGetValue(month.Month, out var feedback);

                monthlyRatingDictionary.TryGetValue(month.Month, out var rating);

                decimal completion = month.ClosedCases == 0
                    ? 0
                    : Math.Round(
                        ((decimal)(feedback?.FeedbackReceived ?? 0)
                        / month.ClosedCases) * 100, 2);

                model.MonthlyTrend.Add(new MonthlyTrendViewModel
                {
                    Month = new DateTime(model.Year.Value, month.Month, 1)
                        .ToString("MMM"),

                    ClosedCases = month.ClosedCases,

                    FeedbackReceived = feedback?.FeedbackReceived ?? 0,

                    CompletionPercentage = completion,

                    AverageRating = Math.Round(
                        rating?.AverageRating ?? 0,
                        2)
                });
            }

            return model;
        }

        public async Task<FileResult> ExportToExcelAsync(int? month, int? year)
        {
            var model = new ServiceFeedbackAnalyticsViewModel
            {
                Month = month,
                Year = year
            };

            model = await GetAnalyticsAsync(model);

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Analytics");

            int row = 1;

            worksheet.Cell(row, 1).Value = "Service Feedback Analytics";
            worksheet.Range(row, 1, row, 6).Merge();
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Font.FontSize = 16;

            row += 2;

            worksheet.Cell(row, 1).Value = "Month";
            worksheet.Cell(row, 2).Value =
                new DateTime(model.Year.Value, model.Month.Value, 1)
                .ToString("MMMM yyyy");

            row += 2;

            worksheet.Cell(row, 1).Value = "Total Closed Cases";
            worksheet.Cell(row, 2).Value = model.TotalClosedCases;

            row++;

            worksheet.Cell(row, 1).Value = "Feedback Received";
            worksheet.Cell(row, 2).Value = model.TotalFeedbackReceived;

            row++;

            worksheet.Cell(row, 1).Value = "Completion %";
            worksheet.Cell(row, 2).Value = model.CompletionPercentage;

            row++;

            worksheet.Cell(row, 1).Value = "Average Rating";
            worksheet.Cell(row, 2).Value = model.AverageRating;

            row += 3;

            worksheet.Cell(row, 1).Value = "Region";
            worksheet.Cell(row, 2).Value = "Closed Cases";
            worksheet.Cell(row, 3).Value = "Feedback Received";
            worksheet.Cell(row, 4).Value = "Completion %";
            worksheet.Cell(row, 5).Value = "Average Rating";
            worksheet.Cell(row, 6).Value = "Status";

            worksheet.Range(row, 1, row, 6).Style.Font.Bold = true;

            row++;

            foreach (var item in model.Regions)
            {
                worksheet.Cell(row, 1).Value = item.Region;
                worksheet.Cell(row, 2).Value = item.ClosedCases;
                worksheet.Cell(row, 3).Value = item.FeedbackReceived;
                worksheet.Cell(row, 4).Value = item.CompletionPercentage;
                worksheet.Cell(row, 5).Value = item.AverageRating;
                worksheet.Cell(row, 6).Value = item.Status;

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            stream.Position = 0;

            return new FileContentResult(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName =
                    $"ServiceFeedbackAnalytics_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            };
        }

        public async Task<List<RegionDetailsViewModel>> GetRegionDetailsAsync(
      string region,
      int? month,
      int? year)
        {
            month ??= DateTime.Now.Month;
            year ??= DateTime.Now.Year;

            var result = await
            (
                from closed in _context.ClosedCaseUploadDetails.AsNoTracking()

                join feedback in _context.CustomerFeedbacks.AsNoTracking()
                    on closed.CaseNumber equals feedback.CaseNumber
                    into fb

                from feedback in fb.DefaultIfEmpty()

                join rating in _context.CustomerFeedbackRatings.AsNoTracking()
                    on feedback.FeedbackID equals rating.FeedbackID
                    into rt

                from rating in rt.DefaultIfEmpty()

                where closed.ExcelClosedDate.HasValue
                      && closed.ExcelClosedDate.Value.Month == month
                      && closed.ExcelClosedDate.Value.Year == year
                      && closed.Region == region

                orderby closed.ExcelClosedDate descending

                select new RegionDetailsViewModel
                {
                    CaseNumber = closed.CaseNumber,

                    CompanyName = feedback != null
                        ? feedback.CompanyName
                        : string.Empty,

                    RespondentName = feedback != null
                        ? feedback.RespondentName
                        : string.Empty,

                    YILEngineer = feedback != null
                        ? feedback.YILEngineer
                        : string.Empty,

                    ClosedDate = closed.ExcelClosedDate,

                    FeedbackReceived = feedback != null,

                    Rating = rating != null
                        ? rating.RatingValue
                        : null
                }

            ).ToListAsync();

            return result;
        }
        private string GetStatus(decimal completionPercentage)
        {
            if (completionPercentage >= 100)
                return "Outstanding";

            if (completionPercentage >= 95)
                return "Excellent";

            if (completionPercentage >= 90)
                return "Good";

            return "Needs Attention";
        }

    }
}