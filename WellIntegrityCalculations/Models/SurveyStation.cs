namespace WellIntegrityCalculations.Models
{
    /// <summary>
    /// Representation of a survey station. Used together with all the survey to interpolate using tangent method and calculate TVDs.
    /// </summary>
    public class SurveyStation
    {
        /// <summary>
        /// Measured depth (fts)
        /// </summary>
        public double Md {get; set;}

        /// <summary>
        /// Inclination (Degrees)
        /// </summary>
        public double? Inc { get; set; }

        /// <summary>
        /// Azimuth (Degrees)
        /// </summary>
        public double? Azi { get; set; }

        /// <summary>
        /// True Vertical Depth (fts)
        /// </summary>
        public double Tvd { get; set; }
        public double? Ns { get; set; }
        public double? Ew { get; set; }
    }
}