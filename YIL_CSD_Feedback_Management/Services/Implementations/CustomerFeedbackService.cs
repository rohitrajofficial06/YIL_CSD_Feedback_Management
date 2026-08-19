using System.IO;
using Microsoft.AspNetCore.Hosting;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Services.Interfaces;
using YIL_CSD_Feedback_Management.ViewModels;
using YIL_CSD_Feedback_Management.Helpers;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class CustomerFeedbackService : ICustomerFeedbackService
    {
        private readonly ICustomerFeedbackRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public CustomerFeedbackService(
            ICustomerFeedbackRepository repository,
            IWebHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
        }

        public async Task<long> SaveAsync(
     CustomerFeedbackViewModel model,
     string createdBy)
        {
            System.Diagnostics.Debug.WriteLine($"Part1 : {model.CaseNumberPart1}");
            System.Diagnostics.Debug.WriteLine($"Part2 : {model.CaseNumberPart2}");
            System.Diagnostics.Debug.WriteLine($"CaseNo: {model.CaseNumber}");
            if (model.DepartmentID == 1)
            {
                model.CaseNumber =
                    $"YIL-C{model.CaseNumberPart1}-{model.CaseNumberPart2}";

                model.ReferenceType = "CASE";
            }
            else if (model.DepartmentID == 2)
            {
                model.CaseNumber = model.ReferenceNumber;

                model.ReferenceType = "TRN";
            }
            else if (model.DepartmentID == 3)
            {
                model.CaseNumber = model.ReferenceNumber;

                model.ReferenceType = "SRN";
            }

            CustomerFeedback feedback = new CustomerFeedback
            {
                DepartmentID = model.DepartmentID,

                CaseNumber = model.CaseNumber,

                ReferenceType = model.ReferenceType,

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

                Region = model.Region,        

                Comments = model.Comments,

                CreatedBy = createdBy,

                CreatedDate = DateTimeHelper.Now
            };

            //==========================================================
            // Save Feedback Attachment
            //==========================================================

            if (model.FeedbackFile != null &&
                model.FeedbackFile.Length > 0)
            {
                string extension =
                    Path.GetExtension(model.FeedbackFile.FileName)
                        .ToLowerInvariant();

                string[] allowedExtensions =
                {
        ".pdf",
        ".jpg",
        ".jpeg",
        ".png"
    };

                if (!allowedExtensions.Contains(extension))
                {
                    throw new InvalidOperationException(
                        "Invalid feedback attachment format.");
                }

                if (model.FeedbackFile.Length > 5 * 1024 * 1024)
                {
                    throw new InvalidOperationException(
                        "Feedback attachment size cannot exceed 5 MB.");
                }

                string uploadFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "feedback");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string uniqueFileName =
                    $"{Guid.NewGuid():N}{extension}";

                string filePath = Path.Combine(
                    uploadFolder,
                    uniqueFileName);

                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await model.FeedbackFile.CopyToAsync(stream);
                }

                feedback.FeedbackFileName =
                    Path.GetFileName(model.FeedbackFile.FileName);

                feedback.FeedbackFilePath =
                    $"/uploads/feedback/{uniqueFileName}";
            }

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

        public async Task<bool> CaseNumberExistsAsync(string caseNumber)
        {
            return await _repository.CaseNumberExistsAsync(caseNumber);
        }

        public async Task<CustomerFeedback?> GetForEditAsync(
    long feedbackId,
    string createdBy)
        {
            return await _repository.GetForEditAsync(
                feedbackId,
                createdBy);
        }

        public async Task UpdateAsync(CustomerFeedback feedback)
        {
            await _repository.UpdateAsync(feedback);
        }

    }
}