namespace WellIntegrityCalculations.Models
{
    /// <summary>
    /// Contains information about production assembly accessories. Only accessories that are relevant for calculations should be included. P.E The ones that have Collapse & Rating pressure
    /// </summary>
    public class Accessory
    {
        /// <summary>
        /// Accessory Name
        /// </summary>
        public string? Accesorio { get; set; }
        
        /// <summary>
        /// Depth in MD of the accessory
        /// </summary>
        public double Profundidad { get; set; }
        
        /// <summary>
        /// SectionType of Accessory
        /// </summary>
        public string? Tipo { get; set; }
        
        /// <summary>
        /// Top Rating Pressure of the component
        /// </summary>
        public double RatingDePresion { get; set; }
        
        /// <summary>
        /// Assembly name in which the Accessory is located
        /// </summary>
        public string? AssemblyAlQuePertenece { get; set; }
        
        /// <summary>
        /// Collapse Pressure value for component
        /// </summary>
        public double CollapsePressure { get; set; }
        
        /// <summary>
        /// Component Type code
        /// </summary>
        public string? CompType { get; set; }
    }
}