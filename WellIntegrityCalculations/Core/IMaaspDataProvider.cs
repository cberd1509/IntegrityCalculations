using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Core
{
    public interface IMaaspDataProvider
    {
        Dictionary<string, double> GetAnnulusA(List<CalculationElement> calculationRulesList, Dictionary<string, double> securityFactors, DatumData datumData);
        Dictionary<string, double> GetAnnulusB(List<CalculationElement> calculationRulesList, Dictionary<string, double> securityFactors, DatumData datumData);
        Dictionary<string, double> GetAnnulusC(List<CalculationElement> calculationRulesList, Dictionary<string, double> securityFactors, DatumData datumData);
    }
}