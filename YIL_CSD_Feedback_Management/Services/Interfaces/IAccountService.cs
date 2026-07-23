using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Services.Interfaces
{
    public interface IAccountService
    {
        Task<User?> LoginAsync(string userName, string password);
    }
}