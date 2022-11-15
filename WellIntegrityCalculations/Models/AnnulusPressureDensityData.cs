namespace WellIntegrityCalculations.Models
{
    public class AnnulusPressureDensityData
    {
        public string AnnulusName { get; set; }
        public int AnnulusIndex { get; set; }
        /// <summary>
        /// Annulus Presure (PSI)
        /// </summary>
        public double Pressure { get; set; }
        /// <summary>
        /// Density (ppg)
        /// </summary>
        public double Density { get; set; }
    }
}