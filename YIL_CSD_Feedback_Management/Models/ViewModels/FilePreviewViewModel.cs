namespace YIL_CSD_Feedback_Management.Models.ViewModels
{
    public class FilePreviewViewModel
    {
        public string FileName { get; set; } = "";

        public string Extension { get; set; } = "";

        public long Size { get; set; }

        public string PreviewUrl { get; set; } = "";
    }
}