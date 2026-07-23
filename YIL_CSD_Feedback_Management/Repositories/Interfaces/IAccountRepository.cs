using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        /// <summary>
        /// Validates the user credentials.
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="password">Password</param>
        /// <returns>User object if credentials are valid; otherwise null.</returns>
        Task<User?> ValidateUserAsync(string userName, string password);

        /// <summary>
        /// Updates the user's last login time.
        /// </summary>
        /// <param name="userId">User ID</param>
        Task UpdateLastLoginAsync(long userId);
    }
}