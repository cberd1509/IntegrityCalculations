namespace WellIntegrityCalculations.Models
{
    /// <summary>
    /// Represents a well perforation information
    /// </summary>
    public class Perforation
    {
        /// <summary>
        /// End MD of interval
        /// </summary>
        public double EndMD { get; set; }
        
        /// <summary>
        /// Start MD of interval
        /// </summary>
        public double StartMD { get; set; }

        /// <summary>
        /// Status of perforations (OPEN/CLOSED)
        /// </summary>
        public string Status { get; set; }
    }
}