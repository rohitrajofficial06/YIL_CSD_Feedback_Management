using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels.User;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Helpers;
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

        public async Task<UserIndexViewModel> GetUsersAsync(
    int departmentId,
    string? searchText,
    string? region,
    string? role,
    bool? isActive,
    string? onlineStatus)
        {
            //=========================================================
            // Current time
            //=========================================================

            DateTime onlineCutoff =
                DateTimeHelper.Now.AddMinutes(-5);


            //=========================================================
            // Base User Query
            //=========================================================

            var query = _context.Users
                .Where(x => x.DepartmentId == departmentId);


            //=========================================================
            // Search
            //=========================================================

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();

                query = query.Where(x =>
                    (x.UserName != null &&
                     x.UserName.Contains(searchText))

                    ||

                    (x.FullName != null &&
                     x.FullName.Contains(searchText))

                    ||

                    (x.Email != null &&
                     x.Email.Contains(searchText)));
            }


            //=========================================================
            // Region
            //=========================================================

            if (!string.IsNullOrWhiteSpace(region))
            {
                region = region.Trim();

                query = query.Where(x =>
                    x.Region == region);
            }


            //=========================================================
            // Role
            //=========================================================

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(x =>
                    x.Role == role);
            }


            //=========================================================
            // Account Active / Inactive
            //=========================================================

            if (isActive.HasValue)
            {
                query = query.Where(x =>
                    x.IsActive == isActive.Value);
            }


            //=========================================================
            // Online / Offline Filter
            //
            // Online means:
            // IsActive = 1 in session table
            // AND LastActivityDate is within last 30 minutes
            //=========================================================

            if (string.Equals(
                onlineStatus,
                "Online",
                StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x =>
                    x.IsActive
                    &&
                    _context.UserActiveSessions.Any(s =>
                        s.UserName == x.UserName
                        &&
                        s.IsActive
                        &&
                        s.LastActivityDate >= onlineCutoff));
            }
            else if (string.Equals(
                onlineStatus,
                "Offline",
                StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x =>
                    !_context.UserActiveSessions.Any(s =>
                        s.UserName == x.UserName
                        &&
                        s.IsActive
                        &&
                        s.LastActivityDate >= onlineCutoff));
            }


            //=========================================================
            // Summary Counts
            //
            // These should be calculated BEFORE pagination.
            //=========================================================

            int totalUsers = await _context.Users
                .Where(x =>
                    x.DepartmentId == departmentId
                    &&
                    x.IsActive)
                .CountAsync();


            int onlineUsers = await _context.Users
                .Where(x =>
                    x.DepartmentId == departmentId
                    &&
                    x.IsActive
                    &&
                    _context.UserActiveSessions.Any(s =>
                        s.UserName == x.UserName
                        &&
                        s.IsActive
                        &&
                        s.LastActivityDate >= onlineCutoff))
                .CountAsync();


            int offlineUsers = totalUsers - onlineUsers;


            //=========================================================
            // User List
            //=========================================================

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

                    CreatedDate = x.CreatedDate,

                    //=================================================
                    // Online Status
                    //=================================================

                    IsOnline =
                        x.IsActive
                        &&
                        _context.UserActiveSessions.Any(s =>
                            s.UserName == x.UserName
                            &&
                            s.IsActive
                            &&
                            s.LastActivityDate >= onlineCutoff),

                    //=================================================
                    // Last Activity
                    //=================================================

                    LastActivityDate =
                        _context.UserActiveSessions
                            .Where(s =>
                                s.UserName == x.UserName)
                            .OrderByDescending(s =>
                                s.LastActivityDate)
                            .Select(s =>
                                (DateTime?)s.LastActivityDate)
                            .FirstOrDefault()
                })
                .ToListAsync();


            //=========================================================
            // Return View Model
            //=========================================================

            return new UserIndexViewModel
            {
                Users = users,

                SearchText = searchText,

                Region = region,

                Role = role,

                IsActive = isActive,

                OnlineStatus = onlineStatus,

                TotalUsers = totalUsers,

                OnlineUsers = onlineUsers,

                OfflineUsers = offlineUsers
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
                CreatedDate = DateTimeHelper.Now,
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