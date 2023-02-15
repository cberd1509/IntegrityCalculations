
using WellIntegrityCalculations.Core;
using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Services
{
    public class CalculationService : ICalculationService
    {

        private readonly ILogger<ICalculationService> _logger;
        private readonly ICalculationRulesProvider _rulesProvider;
        private readonly IMawopDataProvider _mawopDataProvider;
        private readonly IMaaspDataProvider _maaspDataProvider;

        public CalculationService(ILogger<ICalculationService> logger, IMawopDataProvider mawopDataProvider, ICalculationRulesProvider calculationsRulesProvider, IMaaspDataProvider maaspDataProvider)
        {
            _logger = logger;
            _rulesProvider = calculationsRulesProvider;
            _mawopDataProvider = mawopDataProvider;
            _maaspDataProvider = maaspDataProvider;
        }
        public GenericAPIResponseDTO GetWellMawop(WellPressureCalculationRequestDTO requestData)
        {
            List<CalculationElement> calculationRulesList = _rulesProvider.GetCalculationElements(requestData);
            _logger.LogInformation("Rules were determined correctly, starting calculations for each annulus");

            var responseValues = new List<WellPressureCalculationResult>();

            if (requestData.anulares.ToList().Find(x => x.Anular == "Anular A") != null)
            {
                _logger.LogInformation("Calculating Pressures for Annulus A");
                Dictionary<string, double> annulusAMawopValues = _mawopDataProvider.GetAnnulusA(calculationRulesList);
                Dictionary<string, double> annulusAMaaspValues = _maaspDataProvider.GetAnnulusA(calculationRulesList, requestData.SecurityFactors, requestData.ReferenceDepths);

                responseValues.Add(new WellPressureCalculationResult() { Annulus = "Anular A", MaaspValues = annulusAMaaspValues, MawopValues = annulusAMawopValues});
            }

            if (requestData.anulares.ToList().Find(x => x.Anular == "Anular B") != null)
            {
                _logger.LogInformation("Calculating Pressures for Annulus B");
                Dictionary<string, double> annulusBMawopValues = _mawopDataProvider.GetAnnulusB(calculationRulesList);
                Dictionary<string, double> annulusBMaaspValues = _maaspDataProvider.GetAnnulusB(calculationRulesList, requestData.SecurityFactors, requestData.ReferenceDepths);

                responseValues.Add(new WellPressureCalculationResult() { Annulus = "Anular B", MaaspValues = annulusBMaaspValues, MawopValues = annulusBMawopValues});
            }

            if (requestData.anulares.ToList().Find(x => x.Anular == "Anular C") != null)
            {
                _logger.LogInformation("Calculating Pressures for Annulus C");
                Dictionary<string, double> annulusCMawopValues = _mawopDataProvider.GetAnnulusC(calculationRulesList);
                Dictionary<string, double> annulusCMaaspValues = _maaspDataProvider.GetAnnulusC(calculationRulesList, requestData.SecurityFactors, requestData.ReferenceDepths);

                responseValues.Add(new WellPressureCalculationResult() { Annulus = "Anular C", MaaspValues = annulusCMaaspValues, MawopValues = annulusCMawopValues });
            }

            return new GenericAPIResponseDTO()
            {
                StatusCode = 200,
                ResponseValue = responseValues
            };
        }

    }
}
