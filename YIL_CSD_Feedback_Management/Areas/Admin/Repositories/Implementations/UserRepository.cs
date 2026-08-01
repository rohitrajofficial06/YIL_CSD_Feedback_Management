using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels.User;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserIndexViewModel> GetUsersAsync(int departmentId, string? searchText, string? region, string? role, bool? isActive)
        {
            var query = _context.Users
                .Where(x => x.DepartmentId == departmentId);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(x =>
                    (x.UserName != null && x.UserName.Contains(searchText)) ||
                    (x.FullName != null && x.FullName.Contains(searchText)) ||
                    (x.Email != null && x.Email.Contains(searchText)));
            }

            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(x => x.Region == region);
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(x => x.Role == role);
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var users = await query
                .OrderBy(x => x.FullName)
                .Select(x => new UserListViewModel
                {
                    UserID = x.UserID,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Email = x.Email,
                    Role = x.Role,
                    Region = x.Region,
                    IsActive = x.IsActive,
                    LastLogin = x.LastLogin,
                    CreatedDate = x.CreatedDate
                })
                .ToListAsync();

            return new UserIndexViewModel
            {
                Users = users,
                SearchText = searchText,
                Region = region,
                Role = role,
                IsActive = isActive
            };
        }

        public async Task<UserDetailsViewModel?> GetDetailsAsync(long userId, int departmentId)
        {
            return await _context.Users
                .Where(x => x.UserID == userId &&
                            x.DepartmentId == departmentId)
                .Select(x => new UserDetailsViewModel
                {
                    UserID = x.UserID,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Email = x.Email,
                    Role = x.Role,
                    Region = x.Region,
                    IsActive = x.IsActive,
                    LastLogin = x.LastLogin,
                    CreatedDate = x.CreatedDate,
                    Module = x.Module
                })
                .FirstOrDefaultAsync();
        }

        public async Task<UserEditViewModel?> GetEditAsync(long userId, int departmentId)
        {
            return await _context.Users
                .Where(x => x.UserID == userId &&
                            x.DepartmentId == departmentId)
                .Select(x => new UserEditViewModel
                {
                    UserID = x.UserID,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Email = x.Email,
                    Role = x.Role,
                    Region = x.Region,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(UserCreateViewModel model, int departmentId, string module)
        {
            var user = new User
            {
                UserName = model.UserName,
                Password = model.Password,
                FullName = model.FullName,
                Email = model.Email,
                Role = model.Role,
                Region = model.Region,
                IsActive = model.IsActive,
                CreatedDate = DateTime.Now,
                DepartmentId = departmentId,
                Module = module
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserEditViewModel model, int departmentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x =>
                x.UserID == model.UserID &&
                x.DepartmentId == departmentId);

            if (user == null)
                return;

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Role = model.Role;
            user.Region = model.Region;
            user.IsActive = model.IsActive;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.Password = model.Password;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long userId, int departmentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x =>
                x.UserID == userId &&
                x.DepartmentId == departmentId);

            if (user == null)
                return;

            user.IsActive = false;

            await _context.SaveChangesAsync();
        }

    }
}