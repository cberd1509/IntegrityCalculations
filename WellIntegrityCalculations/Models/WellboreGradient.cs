namespace WellIntegrityCalculations.Models
{

    /// <summary>
    /// General representation of a Wellbore Gradient. Can either be Pore or Fracture gradient
    /// </summary>
    public class WellboreGradient
    {

        /// <summary>
        /// Name of the formation
        /// </summary>
        public string formationname { get; set; }
        
        /// <summary>
        /// Value of the gradient in psi/ft
        /// </summary>
        public double value { get; set; }
        
        /// <summary>
        /// MD of gradient measurement
        /// </summary>
        public double depth_md { get; set; }
        
        /// <summary>
        /// TVD of gradient measurement
        /// </summary>
        public double depth_tvd { get; set; }
    }
}