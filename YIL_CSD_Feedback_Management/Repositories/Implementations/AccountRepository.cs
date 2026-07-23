using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;

namespace YIL_CSD_Feedback_Management.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> ValidateUserAsync(string userName, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.UserName == userName &&
                    x.Password == password &&
                    x.IsActive);
        }

        public async Task UpdateLastLoginAsync(long userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user != null)
            {
                user.LastLogin = DateTime.Now;

                _context.Users.Update(user);

                await _context.SaveChangesAsync();
            }
        }
    }
}