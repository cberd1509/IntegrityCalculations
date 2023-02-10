namespace WellIntegrityCalculations.Models
{
    public class DatumData
    {
        public double AirGap { get; set; }
        public double DatumElevation { get; set; }
        public double? Mudline { get; set; }
        public bool? Offshore { get; set; }
        public string? SystemDatum { get; set; }
        public double? WaterDepth { get; set; }
        public double? WellheadDepth { get; set; }
    }
}