using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels.User;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserIndexViewModel> GetUsersAsync(
      int departmentId,
      string? searchText,
      string? region,
      string? role,
      bool? isActive,
      string? onlineStatus);

        Task<UserDetailsViewModel?> GetDetailsAsync(
            long userId,
            int departmentId);

        Task<UserEditViewModel?> GetEditAsync(
            long userId,
            int departmentId);

        Task CreateAsync(
            UserCreateViewModel model,
            int departmentId,
            string module);

        Task UpdateAsync(
            UserEditViewModel model,
            int departmentId);

        Task DeleteAsync(
            long userId,
            int departmentId);
    }
}