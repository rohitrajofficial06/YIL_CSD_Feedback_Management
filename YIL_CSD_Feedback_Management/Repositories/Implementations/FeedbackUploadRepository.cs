using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Repositories.Interfaces;

namespace YIL_CSD_Feedback_Management.Repositories.Implementations
{
    public class FeedbackUploadRepository : IFeedbackUploadRepository
    {
        private readonly ApplicationDbContext _context;

        public FeedbackUploadRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FeedbackUpload>> GetAllAsync()
        {
            return await _context.FeedbackUploads
                                 .OrderByDescending(x => x.UploadID)
                                 .ToListAsync();
        }

        public async Task<FeedbackUpload?> GetByIdAsync(long id)
        {
            return await _context.FeedbackUploads
                                 .FirstOrDefaultAsync(x => x.UploadID == id);
        }

        public async Task AddAsync(FeedbackUpload upload)
        {
            await _context.FeedbackUploads.AddAsync(upload);
        }

        public async Task UpdateAsync(FeedbackUpload upload)
        {
            _context.FeedbackUploads.Update(upload);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(FeedbackUpload upload)
        {
            _context.FeedbackUploads.Remove(upload);
            await Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}