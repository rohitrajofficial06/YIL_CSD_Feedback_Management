using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces
{
    public interface IClosedCasesRepository
    {
        Task<ClosedCasesViewModel> GetPageAsync(
            ClosedCasesViewModel filter);
    }
}