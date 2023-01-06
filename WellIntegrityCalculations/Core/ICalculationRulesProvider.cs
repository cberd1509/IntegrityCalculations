using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Core
{
    public interface ICalculationRulesProvider
    {
        List<CalculationElement> GetCalculationElements(WellPressureCalculationRequestDTO data);
    }
}