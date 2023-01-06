using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Core
{
    public interface IMawopDataProvider
    {
        KeyValuePair<string, double> GetAnnulusA(List<CalculationElement> calculationRulesList);
        KeyValuePair<string, double> GetAnnulusB(List<CalculationElement> calculationRulesList);
        KeyValuePair<string, double> GetAnnulusC(List<CalculationElement> calculationRulesList);
        KeyValuePair<string, double> GetAnnulusD(List<CalculationElement> calculationRulesList);
    }
}