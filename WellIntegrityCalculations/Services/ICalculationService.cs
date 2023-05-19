using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Services
{
    public interface ICalculationService
    {
        GenericAPIResponseDTO<List<WellPressureCalculationResult>> GetWellMawop(WellPressureCalculationRequestDTO requestData);
    }
}