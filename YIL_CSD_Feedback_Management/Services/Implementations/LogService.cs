using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text;
using YIL_CSD_Feedback_Management.Helpers;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class LogService : ILogService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogService(
      IWebHostEnvironment environment,
      IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task WriteLogAsync(
     string level,
     string module,
     string action,
     string message)
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;

                var userName = context?.User?.Identity?.Name ?? "Anonymous";

                var department =
                    context?.User?.FindFirst("DepartmentId")?.Value ?? "N/A";

                var ipAddress =
                    context?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

                var browser =
                    context?.Request?.Headers["User-Agent"].ToString() ?? "Unknown";

                var logFolder = Path.Combine(
                    _environment.ContentRootPath,
                    "Logs");

                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }

                var fileName = $"log-{DateTimeHelper.Now:yyyy-MM-dd}.txt";

                var filePath = Path.Combine(logFolder, fileName);

                var builder = new StringBuilder();

                builder.AppendLine("===========================================================");
                builder.AppendLine($"Date         : {DateTimeHelper.Now:dd-MMM-yyyy HH:mm:ss}");
                builder.AppendLine($"Level        : {level}");
                builder.AppendLine($"Module       : {module}");
                builder.AppendLine($"Action       : {action}");
                builder.AppendLine($"User         : {userName}");
                builder.AppendLine($"Department   : {department}");
                builder.AppendLine($"IP Address   : {ipAddress}");
                builder.AppendLine($"Browser      : {browser}");
                builder.AppendLine($"Message      : {message}");
                builder.AppendLine("===========================================================");
                builder.AppendLine();

                await File.AppendAllTextAsync(
                    filePath,
                    builder.ToString());
            }
            catch
            {
            }
        }

        public async Task InformationAsync(string module, string action, string message)
        {
            await WriteLogAsync("Information", module, action, message);
        }

        public async Task WarningAsync(string module, string action, string message)
        {
            await WriteLogAsync("Warning", module, action, message);
        }

        public async Task ErrorAsync(string module, string action, Exception ex)
        {
            await WriteLogAsync(
                "Error",
                module,
                action,
                ex.ToString());
        }

        public async Task LoginAsync(string username, string department)
        {
            await WriteLogAsync(
                "Login",
                "Account",
                "Login",
                $"User '{username}' logged in successfully. Department : {department}");
        }

        public async Task LogoutAsync(string username)
        {
            await WriteLogAsync(
                "Logout",
                "Account",
                "Logout",
                $"User '{username}' logged out.");
        }
    }
}