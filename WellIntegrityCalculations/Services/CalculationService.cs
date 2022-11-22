using WellIntegrityCalculations.Core;
using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Services
{
    public class CalculationService : ICalculationService
    {

        ILogger<ICalculationService> _logger;
        CalculationRulesProvider _rulesProvider;
        public CalculationService(ILogger<ICalculationService> logger)
        {
            _logger = logger;
            _rulesProvider = new CalculationRulesProvider(_logger);
        }
        public void GetWellMawop(MawopCalculationRequestDTO requestData)
        {
            List<Annulus> annulusList = SchematicHelperFunctions.GetAnnulusContents(requestData.CasingData);
            List<CalculationElement> calculationElements = new List<CalculationElement>();            
        }
      
    }
}
