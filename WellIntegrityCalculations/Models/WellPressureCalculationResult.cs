namespace WellIntegrityCalculations.Models
{
    public class WellPressureCalculationResult
    {
        public string Annulus { get; set; }
        public KeyValuePair<string,double> MaaspValue { get; set; }
        public KeyValuePair<string, double> MawopValue { get; set; }
        public double MopValue { get; set; }
    }
}