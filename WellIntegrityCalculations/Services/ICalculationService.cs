using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Services
{
    public interface ICalculationService
    {
        public GenericAPIResponseDTO GetWellMawop();
        public bool IsSumOdd(int x, int y);
    }
}
