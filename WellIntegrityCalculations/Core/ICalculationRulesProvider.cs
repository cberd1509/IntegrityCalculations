using WellIntegrityCalculations.Models;
using WellIntegrityCalculations.Services;

namespace WellIntegrityCalculations.Core
{
    public interface ICalculationRulesProvider
    {
        public CalculationElement GetInnerWeakestElementInAnnulus(List<Annulus> annulusList , List<AnnulusPressureDensityData> annulusDensities);
        public List<CalculationElement> GetExternalCasingAnalysis(List<Annulus> annulusList, List<AnnulusPressureDensityData> annulusPressureDensity, List<DepthGradient> fracturePressureGradient, List<CementJob> cementJobs);
    }
}