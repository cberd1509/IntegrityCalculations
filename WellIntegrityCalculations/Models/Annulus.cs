namespace WellIntegrityCalculations.Models
{
    /// <summary>
    /// Represents an Annulus element with all of its components and information
    /// </summary>
    public class Annulus
    {

        /// <summary>
        /// Index of the Annulus
        /// </summary>
        public int index { get; set; }

        /// <summary>
        /// Annulus Name
        /// </summary>
        public string Anular { get; set; }

        /// <summary>
        /// Annulus density in PPG
        /// </summary>
        public double Densidad { get; set; }

        /// <summary>
        /// Annulus Pressure in PSI
        /// </summary>
        public double Presion { get; set; }

        /// <summary>
        /// Inner boundary tubulars of annulus
        /// </summary>
        public IList<Tubular>? InnerBoundary { get; set; }

        /// <summary>
        /// Outer boundary tubulars of annulus
        /// </summary>
        public IList<Tubular>? OuterBoundary { get; set; }
    }
}
