namespace WellIntegrityCalculations.Models
{
    public class WellboreGradient
    {
        public string formationname { get; set; }
        public double value { get; set; }
        public double depth_md { get; set; }
        public double depth_tvd { get; set; }
    }
}