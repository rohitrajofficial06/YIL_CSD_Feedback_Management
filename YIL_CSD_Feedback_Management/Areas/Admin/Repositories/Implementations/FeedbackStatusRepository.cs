using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Models;
using static YIL_CSD_Feedback_Management.Areas.Admin.ViewModels.FeedbackStatusUploadViewModel;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations
{
    public class FeedbackStatusRepository : IFeedbackStatusRepository
    {
        private readonly ApplicationDbContext _context;

        public FeedbackStatusRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FeedbackStatusUploadViewModel> GetPageAsync()
        {
            FeedbackStatusUploadViewModel model = new();

            model.UploadHistory = await _context.FeedbackStatusUploadHeaders

                .OrderByDescending(x => x.UploadedDate)

                .Select(x => new FeedbackStatusUploadHeaderViewModel
                {
                    UploadHeaderID = x.UploadHeaderID,

                    FileName = x.FileName,

                    StatusType = x.StatusType,

                    TotalRecords = x.TotalRecords,

                    UpdatedRecords = x.UpdatedRecords,

                    RejectedRecords = x.RejectedRecords,

                    UploadedBy = x.UploadedBy ?? "",

                    UploadedDate = x.UploadedDate
                })

                .ToListAsync();

            return model;
        }

        public async Task<long> UploadAsync(IFormFile file, string status, string uploadedBy)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //--------------------------------------
                // Save Physical Excel
                //--------------------------------------

                var savedFile = await SaveExcelFileAsync(file);

                //--------------------------------------
                // Upload Header
                //--------------------------------------

                ClosedCaseUpload upload = new ClosedCaseUpload
                {
                    FileName = savedFile.FileName,

                    FilePath = savedFile.FilePath,

                    UploadedBy = uploadedBy,

                    UploadDate = DateTime.Now,

                    TotalCases = 0,

                    MatchedCases = 0,

                    UnMatchedCases = 0,

                    Processed = false
                };

                _context.ClosedCaseUploads.Add(upload);

                await _context.SaveChangesAsync();

                //--------------------------------------
                // Read Excel
                //--------------------------------------

                using var stream = new MemoryStream();

                await file.CopyToAsync(stream);

                stream.Position = 0;

                using var workbook = new XLWorkbook(stream);

                var worksheet = workbook.Worksheet(1);

                int lastRow = worksheet.LastRowUsed().RowNumber();

                int lastColumn = worksheet.LastColumnUsed().ColumnNumber();

                //--------------------------------------
                // Detect Columns Automatically
                //--------------------------------------

                int caseNumberColumn = 0;

                int caseOwnerOrgColumn = 0;

                int closedDateColumn = 0;

                for (int col = 1; col <= lastColumn; col++)
                {
                    string header = worksheet.Cell(1, col)
                                             .GetString()
                                             .Trim()
                                             .ToLower();

                    //--------------------------------------
                    // Case Number
                    //--------------------------------------

                    if (IsCaseNumberColumn(header))
                    {
                        caseNumberColumn = col;
                    }

                    if (IsCaseOwnerOrgColumn(header))
                    {
                        caseOwnerOrgColumn = col;
                    }

                    if (IsClosedDateColumn(header))
                    {
                        closedDateColumn = col;
                    }
                }

                //--------------------------------------
                // Validation
                //--------------------------------------

                if (caseNumberColumn == 0)
                {
                    throw new Exception(
                        "Case Number column not found in uploaded Excel.");
                }

                if (caseOwnerOrgColumn == 0)
                {
                    throw new Exception(
                        "Case Owner Org column not found in uploaded Excel.");
                }

                //--------------------------------------
                // Read Excel Rows
                //--------------------------------------

                int totalCases = 0;

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    string caseNumber = row.Cell(caseNumberColumn)
                                           .GetString()
                                           .Trim();

                    if (string.IsNullOrWhiteSpace(caseNumber))
                        continue;

                    string caseOwnerOrg = row.Cell(caseOwnerOrgColumn)
                                             .GetString()
                                             .Trim();
                    DateTime? excelClosedDate = null;

                    if (closedDateColumn > 0)
                    {
                        var cell = row.Cell(closedDateColumn);

                        if (!cell.IsEmpty())
                        {
                            if (cell.DataType == XLDataType.DateTime)
                            {
                                excelClosedDate = cell.GetDateTime();
                            }
                            else if (DateTime.TryParse(cell.GetString(), out DateTime dt))
                            {
                                excelClosedDate = dt;
                            }
                        }
                    }

                    ClosedCaseUploadDetail detail = new ClosedCaseUploadDetail
                    {
                        UploadID = upload.UploadID,
                        CaseNumber = caseNumber,
                        CaseOwnerOrg = caseOwnerOrg,
                        ExcelClosedDate = excelClosedDate,
                        Region = GetRegion(caseOwnerOrg),
                        MatchStatus = "Pending",
                        Remarks = ""
                    };

                    _context.ClosedCaseUploadDetails.Add(detail);

                    totalCases++;
                }

                //--------------------------------------
                // Update Upload Header
                //--------------------------------------

                upload.TotalCases = totalCases;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return upload.UploadID;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ClosedCaseDetailsViewModel>
    GetClosedCaseDetailsAsync(long uploadId)
        {
            ClosedCaseDetailsViewModel model = new();

            //------------------------------------
            // Upload Header
            //------------------------------------

            var header = await _context.ClosedCaseUploads
                .FirstOrDefaultAsync(x => x.UploadID == uploadId);

            if (header == null)
                return model;

            model.UploadID = header.UploadID;

            model.FileName = header.FileName;

            model.UploadedBy = header.UploadedBy ?? "";

            model.UploadDate = header.UploadDate;

            model.TotalCases = header.TotalCases;

            model.Processed = header.Processed;

            //------------------------------------
            // Uploaded Records
            //------------------------------------

            var uploadedRows = await _context.ClosedCaseUploadDetails

     .Where(x => x.UploadID == uploadId)

     .OrderBy(x => x.DetailID)

     .ToListAsync();

            foreach (var row in uploadedRows)
            {
                var feedback = await _context.CustomerFeedbacks
      .AsNoTracking()
      .FirstOrDefaultAsync(x => x.CaseNumber == row.CaseNumber);

                ClosedCaseRowViewModel item = new();

                item.DetailID = row.DetailID;

                item.CaseNumber = row.CaseNumber;

                item.Region = row.Region ?? "";

                item.CaseOwnerOrg = row.CaseOwnerOrg ?? "";

                if (feedback == null)
                {
                    item.FeedbackFound = false;

                    item.MatchStatus = "Not Found";

                    item.Remarks = "Feedback not available";
                }
                else
                {
                    item.FeedbackFound = true;

                    item.FeedbackID = feedback.FeedbackID;
                    item.FeedbackStatus = feedback.FeedbackStatus;

                    if (feedback.FeedbackStatus == "Closed")
                    {
                        item.AlreadyClosed = true;

                        item.MatchStatus = "Already Closed";

                        item.Remarks = "Feedback already closed";
                    }
                    else
                    {
                        item.MatchStatus = "Ready";

                        item.Remarks = "Ready to close";
                    }
                }

                model.Records.Add(item);
            }

            model.TotalCases = model.Records.Count;

            model.EastCases =
     model.Records.Count(x => x.Region == "East");

            model.WestCases =
                model.Records.Count(x => x.Region == "West");

            model.NorthCases =
                model.Records.Count(x => x.Region == "North");

            model.SouthCases =
                model.Records.Count(x => x.Region == "South");

            model.GujaratCases =
                model.Records.Count(x => x.Region == "Gujarat");

            model.BHQCases =
                model.Records.Count(x => x.Region == "BHQ");

            model.MatchedCases =
                model.Records.Count(x => x.FeedbackFound);

            model.UnMatchedCases =
                model.Records.Count(x => !x.FeedbackFound);

            model.AlreadyClosedCases =
                model.Records.Count(x => x.AlreadyClosed);

            return model;
        }

        public async Task<ClosedCaseDetailsViewModel> GetRegionDetailsAsync(
    long uploadId,
    string region)
        {
            ClosedCaseDetailsViewModel model = new();

            //----------------------------------------
            // Upload Header
            //----------------------------------------

            var upload = await _context.ClosedCaseUploads
                .FirstOrDefaultAsync(x => x.UploadID == uploadId);

            if (upload == null)
                return model;

            model.UploadID = upload.UploadID;

            model.FileName = upload.FileName;

            model.UploadedBy = upload.UploadedBy ?? "";

            model.UploadDate = upload.UploadDate;

            model.Processed = upload.Processed;

            //----------------------------------------
            // Region Records
            //----------------------------------------

            var rows = await _context.ClosedCaseUploadDetails

                .Where(x => x.UploadID == uploadId)

                .Where(x => x.Region == region)

                .OrderBy(x => x.DetailID)

                .ToListAsync();

            foreach (var row in rows)
            {
                ClosedCaseRowViewModel item = new();

                item.DetailID = row.DetailID;

                item.CaseNumber = row.CaseNumber;

                item.Region = row.Region;

                item.CaseOwnerOrg = row.CaseOwnerOrg;

                item.FeedbackID = row.FeedbackID;

                item.MatchStatus = row.MatchStatus;

                item.Remarks = row.Remarks;

                model.Records.Add(item);
            }

            model.TotalCases = model.Records.Count;

            return model;
        }
        public async Task CloseUploadedCasesAsync(
      long uploadId,
      string closedBy)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                //---------------------------------------
                // Get Upload Header
                //---------------------------------------

                var upload = await _context.ClosedCaseUploads
                    .FirstOrDefaultAsync(x => x.UploadID == uploadId);

                if (upload == null)
                    return;

                //---------------------------------------
                // Already Processed?
                //---------------------------------------

                if (upload.Processed)
                    return;

                //---------------------------------------
                // Get Uploaded Rows
                //---------------------------------------

                var uploadedRows = await _context.ClosedCaseUploadDetails
                    .Where(x => x.UploadID == uploadId)
                    .OrderBy(x => x.DetailID)
                    .ToListAsync();

                //---------------------------------------
                // Summary Counters
                //---------------------------------------

                int matchedCases = 0;

                int unmatchedCases = 0;

                int alreadyClosedCases = 0;

                int closedCases = 0;
                //---------------------------------------
                // Process Uploaded Cases
                //---------------------------------------

                foreach (var row in uploadedRows)
                {
                    var feedback = await _context.CustomerFeedbacks
                        .FirstOrDefaultAsync(x => x.CaseNumber == row.CaseNumber);

                    //---------------------------------------
                    // Feedback Not Found
                    //---------------------------------------

                    if (feedback == null)
                    {
                        row.MatchStatus = "Feedback Not Found";

                        row.Remarks = "Case Number not available in Customer Feedback.";

                        unmatchedCases++;

                        continue;
                    }

                    //---------------------------------------
                    // Save FeedbackID
                    //---------------------------------------

                    row.FeedbackID = feedback.FeedbackID;

                    //---------------------------------------
                    // Already Closed ?
                    //---------------------------------------

                    if (feedback.FeedbackStatus == "Closed")
                    {
                        row.MatchStatus = "Already Closed";

                        row.Remarks = "Feedback already closed.";

                        alreadyClosedCases++;

                        continue;
                    }
                    //---------------------------------------
                    // Close Feedback
                    //---------------------------------------

                    feedback.FeedbackStatus = "Closed";
                    //---------------------------------------
                    // Save Closed Case
                    //---------------------------------------

                    bool alreadyExists = await _context.ClosedCases
                        .AnyAsync(x => x.FeedbackID == feedback.FeedbackID);

                    if (!alreadyExists)
                    {
                        _context.ClosedCases.Add(
                            new ClosedCase
                            {
                                FeedbackID = feedback.FeedbackID,

                                UploadID = upload.UploadID,

                                CaseNumber = feedback.CaseNumber ?? "",

                                Region = row.Region ?? "",

                                ClosedDate = DateTime.Now,

                                ClosedBy = closedBy
                            });
                    }

                    //---------------------------------------
                    // Update Upload Detail
                    //---------------------------------------

                    row.MatchStatus = "Closed Successfully";

                    row.Remarks = "Feedback closed successfully.";

                    matchedCases++;

                    closedCases++;
                }
                upload.TotalCases = uploadedRows.Count;

                upload.MatchedCases = matchedCases;

                upload.UnMatchedCases = unmatchedCases;

                upload.Processed = true;

                upload.ProcessedDate = DateTime.Now;

                //---------------------------------------
                // Save All Changes
                //---------------------------------------

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task CloseRegionCasesAsync(
    long uploadId,
    string region,
    string closedBy)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var upload = await _context.ClosedCaseUploads
                    .FirstOrDefaultAsync(x => x.UploadID == uploadId);

                if (upload == null)
                    return;

                var rows = await _context.ClosedCaseUploadDetails

                    .Where(x => x.UploadID == uploadId)

                    .Where(x => x.Region == region)

                    .ToListAsync();

                foreach (var row in rows)
                {
                    //----------------------------------------
                    // Find Feedback
                    //----------------------------------------

                    var feedback = await _context.CustomerFeedbacks

                        .FirstOrDefaultAsync(x =>
                            x.CaseNumber == row.CaseNumber);

                    if (feedback == null)
                    {
                        row.MatchStatus = "Feedback Not Found";

                        row.Remarks = "Feedback not available.";

                        continue;
                    }

                    //----------------------------------------
                    // Already Closed
                    //----------------------------------------

                    if (feedback.FeedbackStatus == "Closed")
                    {
                        row.MatchStatus = "Already Closed";

                        row.Remarks = "Already Closed.";

                        continue;
                    }

                    //----------------------------------------
                    // Close
                    //----------------------------------------

                    feedback.FeedbackStatus = "Closed";

                    row.FeedbackID = feedback.FeedbackID;

                    row.MatchStatus = "Closed Successfully";

                    row.Remarks = "Closed.";

                    bool exists =
                        await _context.ClosedCases
                            .AnyAsync(x =>
                                x.FeedbackID ==
                                feedback.FeedbackID);

                    if (!exists)
                    {
                        _context.ClosedCases.Add(

                            new ClosedCase
                            {
                                UploadID = uploadId,

                                FeedbackID = feedback.FeedbackID,

                                CaseNumber = feedback.CaseNumber,

                                Region = row.Region,

                                ClosedDate = DateTime.Now,

                                ClosedBy = closedBy
                            });
                    }
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task<List<FeedbackStatusUploadHistoryViewModel>>
    GetUploadDetailsAsync(long uploadHeaderId)
        {
            return await _context.FeedbackStatusUploads

                .Where(x => x.UploadHeaderID == uploadHeaderId)

                .OrderBy(x => x.CaseNumber)

                .Select(x => new FeedbackStatusUploadHistoryViewModel
                {
                    UploadID = x.UploadID,

                    CaseNumber = x.CaseNumber,

                    FeedbackStatus = x.FeedbackStatus,

                    FileName = x.FileName ?? "",

                    UploadedBy = x.UploadedBy ?? "",

                    UploadedDate = x.UploadedDate,

                    Remarks = x.Remarks ?? ""
                })

                .ToListAsync();
        }

        public async Task DeleteUploadAsync(long uploadHeaderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //------------------------------------------
                // Get all uploaded records
                //------------------------------------------

                var uploadDetails = await _context.FeedbackStatusUploads
                    .Where(x => x.UploadHeaderID == uploadHeaderId)
                    .ToListAsync();

                if (!uploadDetails.Any())
                    return;

                //------------------------------------------
                // Get all Case Numbers
                //------------------------------------------

                var caseNumbers = uploadDetails
                    .Select(x => x.CaseNumber)
                    .Distinct()
                    .ToList();

                //------------------------------------------
                // Get all Feedbacks in ONE Query
                //------------------------------------------

                var feedbacks = await _context.CustomerFeedbacks
                    .Where(x => caseNumbers.Contains(x.CaseNumber!))
                    .ToListAsync();

                //------------------------------------------
                // Reset Status
                //------------------------------------------

                foreach (var feedback in feedbacks)
                {
                    feedback.FeedbackStatus = "Open";
                }

                //------------------------------------------
                // Delete Detail Records
                //------------------------------------------

                _context.FeedbackStatusUploads.RemoveRange(uploadDetails);

                //------------------------------------------
                // Delete Header Record
                //------------------------------------------

                var header = await _context.FeedbackStatusUploadHeaders
                    .FirstOrDefaultAsync(x => x.UploadHeaderID == uploadHeaderId);

                if (header != null)
                {
                    _context.FeedbackStatusUploadHeaders.Remove(header);
                }

                //------------------------------------------
                // Save
                //------------------------------------------

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(long uploadId)
        {
            var upload = await _context.FeedbackStatusUploads
                .FirstOrDefaultAsync(x => x.UploadID == uploadId);

            if (upload == null)
                return;

            _context.FeedbackStatusUploads.Remove(upload);

            await _context.SaveChangesAsync();
        }

        private async Task<(string FileName, string FilePath)> SaveExcelFileAsync(IFormFile file)
        {
            string year = DateTime.Now.Year.ToString();

            string month = DateTime.Now.ToString("MMM");   // Jan Feb Mar Apr

            string day = DateTime.Now.ToString("dd");

            string folder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "Uploads",
                "ClosedCases",
                year,
                month,
                day);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string extension = Path.GetExtension(file.FileName);

            string fileName =
                $"ClosedCases_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";

            string fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string dbPath = Path.Combine(
                "Uploads",
                "ClosedCases",
                year,
                month,
                day,
                fileName).Replace("\\", "/");

            return (fileName, dbPath);
        }

        private string GetRegion(string caseOwnerOrg)
        {
            if (string.IsNullOrWhiteSpace(caseOwnerOrg))
                return "";

            caseOwnerOrg = caseOwnerOrg.ToUpper();

            if (caseOwnerOrg.StartsWith("YIL-E"))
                return "East";

            if (caseOwnerOrg.StartsWith("YIL-W"))
                return "West";

            if (caseOwnerOrg.StartsWith("YIL-N"))
                return "North";

            if (caseOwnerOrg.StartsWith("YIL-S"))
                return "South";

            if (caseOwnerOrg.StartsWith("YIL-GJ"))
                return "Gujarat";

            if (caseOwnerOrg.StartsWith("YIL-BHQ"))
                return "BHQ";

            return "";
        }

        private bool IsCaseNumberColumn(string header)
        {
            header = header.Trim().ToLower();

            return header == "case_no."
                || header == "case_no"
                || header == "case no"
                || header == "case number"
                || header == "case_number"
                || header == "casenumber"
                || header == "case";
        }

        private bool IsCaseOwnerOrgColumn(string header)
        {
            header = header.Trim().ToLower();

            return header == "case_owner_org."
                || header == "case_owner_org"
                || header == "case owner org"
                || header == "case owner organization";
        }

        private bool IsClosedDateColumn(string header)
        {
            header = header.Trim().ToLower();

            return header == "closed_date"
                || header == "closed date"
                || header == "closeddate";
        }
    }
}