namespace WellIntegrityCalculations.Models
{
    /// <summary>
    /// Reference Depths information for calculations
    /// </summary>
    public class DatumData
    {
        /// <summary>
        /// Air Gap in fts
        /// </summary>
        public double AirGap { get; set; }
        
        /// <summary>
        /// Datum Elevation in fts
        /// </summary>
        public double DatumElevation { get; set; }
        public double? Mudline { get; set; }
        public bool? Offshore { get; set; }
        public string? SystemDatum { get; set; }
        public double? WaterDepth { get; set; }
        public double? WellheadDepth { get; set; }
    }
}