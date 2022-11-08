namespace WellIntegrityCalculations.Models
{
    public class Annulus
    {
        int AnnulusIndex { get; }
        public List<CasingData> InnerBoundary { get; set; }
        public List<CasingData> OuterBoundary{ get; set; } = new List<CasingData>();

        public Annulus(int annulusIndex, List<CasingData> innerBoundary, List<CasingData> outerBoundary)
        {
            AnnulusIndex = annulusIndex;
            InnerBoundary = innerBoundary;
            OuterBoundary = outerBoundary;
        }

        public Annulus(int annulusIndex)
        {
            AnnulusIndex = annulusIndex;
            InnerBoundary = new List<CasingData>();
            OuterBoundary = new List<CasingData>();
        }
    }
}
