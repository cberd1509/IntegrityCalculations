namespace WellIntegrityCalculations.Models
{
    /// <summary>
    /// Represents a Liner Hanger or similar element information
    /// </summary>
    public class LinerHanger
    {
        /// <summary>
        /// Name of the element
        /// </summary>
        public string? LinerHangerName { get; set; }
        
        /// <summary>
        /// Measured depth of the element
        /// </summary>
        public double ProfundidadMd { get; set; }
        
        /// <summary>
        /// Pressure Rating (psi)
        /// </summary>
        public double RatingDePresion { get; set; }
        
        /// <summary>
        /// Name of the assembly to which it belongs. Used for diferentiating hangers from diferent liner strings.
        /// </summary>
        public string AssemblyAlQuePertenece { get; set; }

        /// <summary>
        /// Element burst pressure (psi)
        /// </summary>
        public double BurstPressure { get; set; }
    }
}