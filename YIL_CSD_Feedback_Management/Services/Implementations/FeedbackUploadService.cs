using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class FeedbackUploadService : IFeedbackUploadService
    {
        // Private Members
        private readonly IFeedbackUploadRepository _repository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IWebHostEnvironment _environment;
        private readonly IOCRService _ocrService;
        private readonly ITemplateDetectionService _templateDetectionService;
        private readonly IFormParserService _formParserService;

        public FeedbackUploadService(
            IFeedbackUploadRepository repository,
            IFileStorageService fileStorageService,
            IWebHostEnvironment environment,
            IOCRService ocrService,
            ITemplateDetectionService templateDetectionService,
            IFormParserService formParserService)
        {
            _repository = repository;
            _fileStorageService = fileStorageService;
            _environment = environment;
            _ocrService = ocrService;
            _templateDetectionService = templateDetectionService;
            _formParserService = formParserService;
        }

        public async Task<long> UploadAsync(IFormFile file, int? departmentId)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Please select a valid feedback file.");

            // Allowed file extensions
            string[] allowedExtensions =
            {
        ".jpg",
        ".jpeg",
        ".png",
        ".pdf"
    };

            string extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Only JPG, JPEG, PNG and PDF files are allowed.");

            // Maximum file size (10 MB)
            const long maxFileSize = 10 * 1024 * 1024;

            if (file.Length > maxFileSize)
                throw new Exception("Maximum file size allowed is 10 MB.");

            // Save file to disk using FileStorageService
            FileUploadResult fileResult = await _fileStorageService.SaveFileAsync(file);

            // Save upload information in database
            FeedbackUpload upload = new FeedbackUpload
            {
                DepartmentID = departmentId,

                OriginalFileName = fileResult.OriginalFileName,

                SavedFileName = fileResult.SavedFileName,

                FilePath = fileResult.RelativePath,

                FileExtension = fileResult.Extension,

                ContentType = fileResult.ContentType,

                FileSize = fileResult.FileSize,

                UploadStatus = "Uploaded",

                OCRStatus = "Pending",

                UploadedDate = fileResult.UploadedOn,

                UploadedBy = "Admin"
            };

            await _repository.AddAsync(upload);

            await _repository.SaveAsync();

            return upload.UploadID;
        }

        public async Task<IEnumerable<FeedbackUpload>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<FeedbackUpload?> GetByIdAsync(long id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<string> ExtractOCRAsync(long uploadId)
        {
            // Get uploaded file details
            var upload = await _repository.GetByIdAsync(uploadId);

            if (upload == null)
                throw new Exception("Uploaded file not found.");

            // Build full physical file path
            string fullPath = Path.Combine(
                _environment.WebRootPath,
                upload.FilePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            // Extract OCR Result
            OCRResult result = await _ocrService.ExtractTextAsync(fullPath);

            // Check OCR Success
            if (!result.IsSuccess)
            {
                throw new Exception(string.Join(Environment.NewLine, result.Warnings));
            }

            // Detect Template
            result.TemplateName = _templateDetectionService.DetectTemplate(result.OCRText);

            // Parse Form
            result.ParsedData = _formParserService.Parse(result.OCRText);

            // Update Upload Information
            upload.OCRStatus = "Completed";
            upload.OCRCompletedDate = DateTime.Now;
            upload.OCRText = result.OCRText;
            upload.TemplateName = result.TemplateName;

            // Save Changes
            await _repository.UpdateAsync(upload);
            await _repository.SaveAsync();

            // Return Extracted Text
            return result.OCRText;
        }
    }
}