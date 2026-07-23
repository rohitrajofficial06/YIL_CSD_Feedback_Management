using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class TemplateDetectionService : ITemplateDetectionService
    {
        public string DetectTemplate(string ocrText)
        {
            if (ocrText.Contains("Training Feedback", StringComparison.OrdinalIgnoreCase))
                return "Training";

            if (ocrText.Contains("Site Service", StringComparison.OrdinalIgnoreCase))
                return "SiteService";

            if (ocrText.Contains("Bench Repair", StringComparison.OrdinalIgnoreCase))
                return "BenchRepair";

            if (ocrText.Contains("Calibration", StringComparison.OrdinalIgnoreCase))
                return "Calibration";

            if (ocrText.Contains("AMC", StringComparison.OrdinalIgnoreCase))
                return "AMC";

            return "Unknown";
        }
    }
}