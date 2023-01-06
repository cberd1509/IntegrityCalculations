
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


            _logger.LogInformation("Calculating Pressures for Annulus A");
            KeyValuePair<string, double> annulusAMawop = _mawopDataProvider.GetAnnulusA(calculationRulesList);
            KeyValuePair<string, double> annulusAMaasp = _maaspDataProvider.GetAnnulusA(calculationRulesList, requestData.SecurityFactors);
            double annulusAMop = annulusAMawop.Value / 2;

            _logger.LogInformation("Calculating Pressures for Annulus B");
            KeyValuePair<string, double> annulusBMawop = _mawopDataProvider.GetAnnulusB(calculationRulesList);
            KeyValuePair<string, double> annulusBMaasp = _maaspDataProvider.GetAnnulusB(calculationRulesList, requestData.SecurityFactors);
            double annulusBMop = annulusBMawop.Value / 2;

            _logger.LogInformation("Calculating Pressures for Annulus C");
            KeyValuePair<string, double> annulusCMawop = _mawopDataProvider.GetAnnulusC(calculationRulesList);
            KeyValuePair<string, double> annulusCMaasp = _maaspDataProvider.GetAnnulusC(calculationRulesList, requestData.SecurityFactors);
            double annulusCMop = annulusCMawop.Value / 2;

            return new GenericAPIResponseDTO()
                {
                    StatusCode = 200,
                    ResponseValue = new List<WellPressureCalculationResult>(){
                    new WellPressureCalculationResult(){ Annulus = "Anular A", MaaspValue= annulusAMaasp, MawopValue = annulusAMawop, MopValue = annulusAMop},
                    new WellPressureCalculationResult(){ Annulus = "Anular B", MaaspValue= annulusBMaasp, MawopValue = annulusBMawop, MopValue = annulusBMop},
                    new WellPressureCalculationResult(){ Annulus = "Anular C", MaaspValue= annulusCMaasp, MawopValue = annulusBMawop, MopValue = annulusCMop},
                }
            };
        }

    }
}
