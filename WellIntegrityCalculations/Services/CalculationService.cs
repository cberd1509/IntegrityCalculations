using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Services
{
    public class CalculationService : ICalculationService
    {

        ILogger<ICalculationService> _logger;
        public CalculationService(ILogger<ICalculationService> logger)
        {
            _logger = logger;
        }

        public GenericAPIResponseDTO GetWellMawop()
        {
            return new GenericAPIResponseDTO { ResponseValue = "Test",StatusCode=200};
        }

        public bool IsSumOdd(int x, int y)
        {
            return (x+y)%2 == 0;
        }
    }
}
