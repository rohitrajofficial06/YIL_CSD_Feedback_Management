using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();

        Task<Department?> GetByIdAsync(int id);

        Task<bool> ExistsAsync(string departmentCode);

        Task AddAsync(Department department);

        Task UpdateAsync(Department department);

        Task DeleteAsync(int id);

        Task SaveAsync();
    }
}