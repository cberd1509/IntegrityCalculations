using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Services
{
    public interface ICalculationService
    {
        GenericAPIResponseDTO GetWellMawop(WellPressureCalculationRequestDTO requestData);
    }
}