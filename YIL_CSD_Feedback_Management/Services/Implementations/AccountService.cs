using System.Security.Claims;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<User?> LoginAsync(string userName, string password)
        {
            var user = await _accountRepository.ValidateUserAsync(userName, password);

            if (user != null)
            {
                await _accountRepository.UpdateLastLoginAsync(user.UserID);
            }

            return user;
        }

       
    }
}