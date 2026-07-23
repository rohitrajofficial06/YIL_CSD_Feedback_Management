using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;

namespace YIL_CSD_Feedback_Management.Repositories.Implementations
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                                 .OrderBy(x => x.DisplayOrder)
                                 .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                                 .FirstOrDefaultAsync(x => x.DepartmentID == id);
        }

        public async Task<bool> ExistsAsync(string departmentCode)
        {
            return await _context.Departments
                .AnyAsync(x => x.DepartmentCode == departmentCode);
        }

        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var department = await GetByIdAsync(id);

            if (department != null)
            {
                _context.Departments.Remove(department);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}