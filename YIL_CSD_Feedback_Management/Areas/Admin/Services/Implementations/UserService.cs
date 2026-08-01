using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels.User;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserIndexViewModel> GetUsersAsync(
            int departmentId,
            string? searchText,
            string? region,
            string? role,
            bool? isActive)
        {
            return await _repository.GetUsersAsync(
                departmentId,
                searchText,
                region,
                role,
                isActive);
        }

        public async Task<UserDetailsViewModel?> GetDetailsAsync(
            long userId,
            int departmentId)
        {
            return await _repository.GetDetailsAsync(
                userId,
                departmentId);
        }

        public async Task<UserEditViewModel?> GetEditAsync(
            long userId,
            int departmentId)
        {
            return await _repository.GetEditAsync(
                userId,
                departmentId);
        }

        public async Task CreateAsync(
            UserCreateViewModel model,
            int departmentId,
            string module)
        {
            await _repository.CreateAsync(
                model,
                departmentId,
                module);
        }

        public async Task UpdateAsync(
            UserEditViewModel model,
            int departmentId)
        {
            await _repository.UpdateAsync(
                model,
                departmentId);
        }

        public async Task DeleteAsync(
            long userId,
            int departmentId)
        {
            await _repository.DeleteAsync(
                userId,
                departmentId);
        }
    }
}