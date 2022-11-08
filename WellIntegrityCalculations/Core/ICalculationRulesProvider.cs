using WellIntegrityCalculations.Models;
using WellIntegrityCalculations.Services;

namespace WellIntegrityCalculations.Core
{
    public interface ICalculationRulesProvider
    {
        public CalculationElement GetInnerWeakestElementInAnnulus(List<Annulus> annulusList , List<DepthGradient> porePressureGradient);
        public List<CalculationElement> GetExternalCasingAnalysis(List<Annulus> annulusList, List<DepthGradient> porePressureGradient, List<DepthGradient> fracturePressureGradient);
    }
}