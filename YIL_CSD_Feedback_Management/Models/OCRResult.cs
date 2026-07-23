using System.Collections.Generic;

namespace YIL_CSD_Feedback_Management.Models
{
    public class OCRResult
    {
        public bool IsSuccess { get; set; }

        public string OCRText { get; set; } = string.Empty;

        public string TemplateName { get; set; } = "Unknown";

        public float Confidence { get; set; }

        public List<string> Warnings { get; set; } = new();

        public ParsedFeedback? ParsedData { get; set; }
    }
}