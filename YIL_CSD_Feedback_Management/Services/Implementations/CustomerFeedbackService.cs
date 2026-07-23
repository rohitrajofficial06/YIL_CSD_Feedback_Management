using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Services.Interfaces;
using YIL_CSD_Feedback_Management.ViewModels;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class CustomerFeedbackService : ICustomerFeedbackService
    {
        private readonly ICustomerFeedbackRepository _repository;

        public CustomerFeedbackService(
            ICustomerFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task<long> SaveAsync(CustomerFeedbackViewModel model)
        {
            model.CaseNumber = $"YIL-C{model.CaseNumberPart1}-{model.CaseNumberPart2}";

            CustomerFeedback feedback = new CustomerFeedback
            {
                DepartmentID = model.DepartmentID,

                CaseNumber = model.CaseNumber,

                FeedbackStatus = "Open",

                FeedbackSource = "Online",

                CompanyName = model.CompanyName,

                RespondentName = model.RespondentName,

                Designation = model.Designation,

                ContactNo = model.ContactNo,

                EmailID = model.EmailID,

                ServiceRequestNo = model.ServiceRequestNo,

                InstrumentCategory = model.InstrumentCategory,

                YILEngineer = model.YILEngineer,

                Comments = model.Comments,

                CreatedBy = "Customer"
            };

            foreach (var item in model.Questions)
            {
                feedback.Ratings.Add(new CustomerFeedbackRating
                {
                    QuestionNo = item.QuestionNo,

                    QuestionText = item.QuestionText,

                    RatingValue = item.RatingValue,

                    IsNotApplicable = item.IsNotApplicable
                });
            }

            await _repository.AddAsync(feedback);

            await _repository.SaveAsync();

            return feedback.FeedbackID;
        }

       
    }
}