using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentService(IDepartmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> SaveAsync(Department department)
        {
            if (await _repository.ExistsAsync(department.DepartmentCode))
                return false;

            await _repository.AddAsync(department);
            await _repository.SaveAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(Department department)
        {
            await _repository.UpdateAsync(department);
            await _repository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            await _repository.SaveAsync();

            return true;
        }
    }
}