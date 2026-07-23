using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Services.Implementations
{
    public class ClosedCasesService : IClosedCasesService
    {
        private readonly IClosedCasesRepository _repository;

        public ClosedCasesService(IClosedCasesRepository repository)
        {
            _repository = repository;
        }

        public async Task<ClosedCasesViewModel> GetPageAsync(ClosedCasesViewModel filter)
        {
            return await _repository.GetPageAsync(filter);
        }
    }
}