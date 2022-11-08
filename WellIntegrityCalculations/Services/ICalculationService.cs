using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Services
{
    public interface ICalculationService
    {
        List<Annulus> GetAnnulusContents(List<CasingData> casingData);
        CasingData GetInnerWeakestElementFromAnnulus(Annulus a);
        CasingData GetOuterWeakestElementFromAnnulus(Annulus a);
        void GetWellMawop(MawopCalculationRequestDTO requestData);
    }
}