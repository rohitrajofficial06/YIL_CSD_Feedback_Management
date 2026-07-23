using System.Text.RegularExpressions;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class FormParserService : IFormParserService
    {
        public ParsedFeedback Parse(string ocrText)
        {
            ParsedFeedback feedback = new ParsedFeedback();

            feedback.ParticipantName = GetValue(ocrText, "Participant Name");
            feedback.CompanyName = GetValue(ocrText, "Company");
            feedback.CustomerName = GetValue(ocrText, "Customer");
            feedback.Phone = GetValue(ocrText, "Phone");
            feedback.Email = GetValue(ocrText, "Email");
            feedback.Course = GetValue(ocrText, "Course");
            feedback.Faculty = GetValue(ocrText, "Faculty");

            feedback.Comments = GetValue(ocrText, "Comments");

            feedback.OverallRating = GetRating(ocrText);

            return feedback;
        }

        private string? GetValue(string text, string key)
        {
            var match = Regex.Match(
                text,
                $"{Regex.Escape(key)}\\s*:?\\s*(.+)",
                RegexOptions.IgnoreCase);

            return match.Success
                ? match.Groups[1].Value.Trim()
                : null;
        }

        private string GetRating(string text)
        {
            if (text.Contains("Excellent", StringComparison.OrdinalIgnoreCase))
                return "Excellent";

            if (text.Contains("Good", StringComparison.OrdinalIgnoreCase))
                return "Good";

            if (text.Contains("Average", StringComparison.OrdinalIgnoreCase))
                return "Average";

            if (text.Contains("Poor", StringComparison.OrdinalIgnoreCase))
                return "Poor";

            return "";
        }
    }
}