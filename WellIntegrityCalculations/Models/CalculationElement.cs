namespace WellIntegrityCalculations.Models
{
    public class CalculationElement
    {
        public CalculationRulesCode RuleCode { get; set; }
        public string? RuleTitle { get; set; }
        public bool IsRelevant { get; set; }
        public double Diameter { get; set; }
        public double ComponentTvd { get; set; }
        public double CasingShoeTvd { get; set; }
        public double TopOfCementInAnular { get; set; }
        public double MaxOperationRatingPressure { get; set; }
        public double CollapsePressure { get; set; }
        public double BurstPressure { get; set; }
        public double PressureGradient { get; set; }
        public double BelowFormationPressureBelow { get; set; }
        public double BelowFormationFractureGradient { get; set; }
        public double BelowFormationDepth { get; set; }
        public double OpenHoleFractureGradient { get; set; }
    }
}
