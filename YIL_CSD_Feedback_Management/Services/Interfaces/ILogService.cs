using System.Threading.Tasks;

namespace YIL_CSD_Feedback_Management.Services.Interfaces
{
    public interface ILogService
    {
        Task InformationAsync(
            string module,
            string action,
            string message);

        Task WarningAsync(
            string module,
            string action,
            string message);

        Task ErrorAsync(
            string module,
            string action,
            Exception ex);

        Task LoginAsync(
            string username,
            string department);

        Task LogoutAsync(
            string username);
    }
}