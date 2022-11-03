using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Services
{
    public class CalculationService : ICalculationService
    {

        ILogger<CalculationService> _logger;
        public CalculationService(ILogger<CalculationService> logger)
        {
            _logger = logger;
        }

        public GenericAPIResponseDTO GetWellMawop()
        {
            return new GenericAPIResponseDTO { ResponseValue = "Test",StatusCode=200};
        }
    }
}
