namespace WellIntegrityCalculations.Models
{
    public class AssemblyComponent
    {
        public string AssemblyType { get; set; }
        public double Diameter { get; set; }
        public double Tvd { get; set; }
        public double MaxOperationPressure { get; set; }
        public double CollapsePressure { get; set; }

    }
}