namespace YIL_CSD_Feedback_Management.Models
{
    public class FileUploadResult
    {
        public string OriginalFileName { get; set; } = string.Empty;

        public string SavedFileName { get; set; } = string.Empty;

        public string RelativePath { get; set; } = string.Empty;

        public string PhysicalPath { get; set; } = string.Empty;

        public string Extension { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedOn { get; set; }
    }
}