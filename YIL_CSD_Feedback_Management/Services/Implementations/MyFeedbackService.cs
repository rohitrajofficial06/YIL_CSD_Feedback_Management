using Microsoft.AspNetCore.Mvc.Rendering;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Services.Interfaces;
using YIL_CSD_Feedback_Management.ViewModels.MyFeedback;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class MyFeedbackService : IMyFeedbackService
    {
        private readonly IMyFeedbackRepository _repository;

        public MyFeedbackService(
            IMyFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task<MyFeedbackSearchViewModel> GetMyFeedbackAsync(
     string createdBy,
     string? searchText,
     string? status,
     string? region,
     DateTime? fromDate,
     DateTime? toDate,
     int pageNumber,
     int pageSize,
     string sortColumn,
     string sortOrder)
        {
            var result = await _repository.GetMyFeedbackAsync(
                createdBy,
                searchText,
                status,
                region,
                fromDate,
                toDate,
                pageNumber,
                pageSize,
                sortColumn,
                sortOrder);

            var feedbacks = result.Feedbacks;

            MyFeedbackSearchViewModel model = new();

            model.SearchText = searchText;
            model.Status = status;
            model.Region = region;
            model.FromDate = fromDate;
            model.ToDate = toDate;

            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.TotalRecords = result.TotalRecords;

            model.SortColumn = sortColumn;
            model.SortOrder = sortOrder;

            //-----------------------------------------
            // Status Dropdown
            //-----------------------------------------

            model.StatusList = new List<SelectListItem>
    {
        new SelectListItem
        {
            Value = "",
            Text = "All Status"
        },

        new SelectListItem
        {
            Value = "Open",
            Text = "Open"
        },

        new SelectListItem
        {
            Value = "Closed",
            Text = "Closed"
        }
    };

            //-----------------------------------------
            // Region Dropdown
            //-----------------------------------------

            model.RegionList.Add(new SelectListItem
            {
                Value = "",
                Text = "All Regions"
            });

            foreach (var item in feedbacks
                .Select(x => x.Region)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x))
            {
                model.RegionList.Add(new SelectListItem
                {
                    Value = item,
                    Text = item
                });
            }

            //-----------------------------------------
            // Grid
            //-----------------------------------------

            model.Feedbacks = feedbacks
                .Select(x => new MyFeedbackListViewModel
                {
                    FeedbackID = x.FeedbackID,

                    CaseNumber = x.CaseNumber ?? "",

                    CompanyName = x.CompanyName,

                    RespondentName = x.RespondentName,

                    Region = x.Region ?? "",

                    FeedbackStatus = x.FeedbackStatus,

                    CreatedDate = x.CreatedDate,

                    AverageRating = x.Ratings
                        .Where(r => !r.IsNotApplicable &&
                                    r.RatingValue.HasValue)
                        .Select(r => r.RatingValue!.Value)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .ToList();

            return model;
        }

        public async Task<MyFeedbackDetailsViewModel?> GetDetailsAsync(
            long feedbackId,
            string createdBy)
        {
            return await _repository.GetDetailsAsync(
                feedbackId,
                createdBy);
        }
    }
}