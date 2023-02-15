namespace WellIntegrityCalculations.Models
{
    public class WellPressureCalculationResult
    {
        public string Annulus { get; set; }
        public Dictionary<string,double> MaaspValues { get; set; }
        public Dictionary<string, double> MawopValues { get; set; }
    }
}