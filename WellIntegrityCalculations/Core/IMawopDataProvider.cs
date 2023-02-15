using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Core
{
    public interface IMawopDataProvider
    {
        Dictionary<string, double> GetAnnulusA(List<CalculationElement> calculationRulesList);
        Dictionary<string, double> GetAnnulusB(List<CalculationElement> calculationRulesList);
        Dictionary<string, double> GetAnnulusC(List<CalculationElement> calculationRulesList);
        Dictionary<string, double> GetAnnulusD(List<CalculationElement> calculationRulesList);
    }
}