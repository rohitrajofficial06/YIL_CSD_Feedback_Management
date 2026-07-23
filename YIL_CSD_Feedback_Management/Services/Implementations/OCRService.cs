using Tesseract;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class OCRService : IOCRService
    {
        private readonly IWebHostEnvironment _environment;

        public OCRService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<OCRResult> ExtractTextAsync(string imagePath)
        {
            return await Task.Run(() =>
            {
                OCRResult result = new OCRResult();

                try
                {
                    string tessDataPath = Path.Combine(
                        _environment.ContentRootPath,
                        "tessdata");

                    using var engine = new TesseractEngine(
                        tessDataPath,
                        "eng",
                        EngineMode.Default);

                    using var image = Pix.LoadFromFile(imagePath);

                    using var page = engine.Process(image);

                    result.IsSuccess = true;
                    result.OCRText = page.GetText()?.Trim() ?? string.Empty;
                    result.Confidence = page.GetMeanConfidence() * 100;

                    // Template and ParsedData will be assigned later
                    result.TemplateName = "Unknown";

                    if (result.Confidence < 70)
                    {
                        result.Warnings.Add(
                            $"Low OCR confidence ({result.Confidence:F2}%). Please verify the extracted data.");
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.OCRText = string.Empty;
                    result.Confidence = 0;
                    result.TemplateName = "Unknown";
                    result.Warnings.Add(ex.Message);

                    return result;
                }
            });
        }
    }
}