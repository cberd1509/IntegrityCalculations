using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Core
{
    public interface IMaaspDataProvider
    {
        KeyValuePair<string, double> GetAnnulusA(List<CalculationElement> calculationRulesList, Dictionary<string, double> securityFactors);
        KeyValuePair<string, double> GetAnnulusB(List<CalculationElement> calculationRulesList, Dictionary<string, double> securityFactors);
        KeyValuePair<string, double> GetAnnulusC(List<CalculationElement> calculationRulesList, Dictionary<string, double> securityFactors);
    }
}