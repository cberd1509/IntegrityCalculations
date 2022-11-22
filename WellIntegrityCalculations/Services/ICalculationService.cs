using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Services
{
    public interface ICalculationService
    {
        void GetWellMawop(MawopCalculationRequestDTO requestData);
    }
}