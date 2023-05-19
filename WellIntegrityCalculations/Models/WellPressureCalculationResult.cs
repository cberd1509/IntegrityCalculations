namespace WellIntegrityCalculations.Models
{
    /// <summary>
    /// Contains the information for a given Annulus of its Maasp and Mawop Values
    /// </summary>
    public class WellPressureCalculationResult
    {
        /// <summary>
        /// Annulus name can either be Anular A, Anular B, or Anular C
        /// </summary>
        public string Annulus { get; set; }

        /// <summary>
        /// Key value pair indicating the MAASP value and its location. P.E { 1A, 2000 } Value is in psi
        /// </summary>
        public Dictionary<string,double> MaaspValues { get; set; }
        /// <summary>
        /// Key value pair indicating the MAWOP value and its location. P.E { 1A, 2000 } Value is in psi
        /// </summary>
        public Dictionary<string, double> MawopValues { get; set; }
    }
}