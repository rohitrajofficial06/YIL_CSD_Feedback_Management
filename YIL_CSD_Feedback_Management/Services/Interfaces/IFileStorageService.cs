using Microsoft.AspNetCore.Http;
using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<FileUploadResult> SaveFileAsync(IFormFile file);

        bool FileExists(string relativePath);

        void DeleteFile(string relativePath);
    }
}