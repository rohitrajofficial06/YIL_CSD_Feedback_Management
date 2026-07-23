using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<FileUploadResult> SaveFileAsync(IFormFile file)
        {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString("00");

            string uploadFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "feedback",
                year,
                month);

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string extension = Path.GetExtension(file.FileName).ToLower();

            string savedFileName = $"{Guid.NewGuid()}{extension}";

            string physicalPath = Path.Combine(uploadFolder, savedFileName);

            using var stream = new FileStream(physicalPath, FileMode.Create);

            await file.CopyToAsync(stream);

            return new FileUploadResult
            {
                OriginalFileName = file.FileName,
                SavedFileName = savedFileName,
                PhysicalPath = physicalPath,
                RelativePath = Path.Combine(
                    "uploads",
                    "feedback",
                    year,
                    month,
                    savedFileName).Replace("\\", "/"),
                Extension = extension,
                ContentType = file.ContentType,
                FileSize = file.Length,
                UploadedOn = DateTime.Now
            };
        }

        public bool FileExists(string relativePath)
        {
            string path = Path.Combine(
                _environment.WebRootPath,
                relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            return File.Exists(path);
        }

        public void DeleteFile(string relativePath)
        {
            string path = Path.Combine(
                _environment.WebRootPath,
                relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}