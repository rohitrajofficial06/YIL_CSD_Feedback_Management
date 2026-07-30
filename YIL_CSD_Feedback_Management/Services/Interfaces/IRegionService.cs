using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Services.Interfaces
{
    public interface IRegionService
    {
        Task<List<Region>> GetAllAsync();
    }
}