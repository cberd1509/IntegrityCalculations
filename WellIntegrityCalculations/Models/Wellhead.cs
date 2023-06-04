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
        /// Pressure rating of the weakest element in wellhead assembly. It can be null if no Wellhead is installed for that section. E.G An abandonment well
        /// </summary>
        public double? RatingDePresion { get; set; }
    }
}