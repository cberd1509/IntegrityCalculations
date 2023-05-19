namespace WellIntegrityCalculations.Models
{
    /// <summary>
    /// Representation of the weakest element in an annulus wellhead.
    /// </summary>
    public class Wellhead
    {
        /// <summary>
        /// Annulus Name
        /// </summary>
        public string Anular { get; set; }
        
        /// <summary>
        /// Pressure rating of the weakest element in wellhead assembly
        /// </summary>
        public double RatingDePresion { get; set; }
    }
}