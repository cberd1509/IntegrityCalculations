using System.Text.Json.Serialization;
using WellIntegrityCalculations.Core;

namespace WellIntegrityCalculations.Models
{
    /// <summary>
    /// Represents a Lithological formation
    /// </summary>
    public class Formation
    {
        /// <summary>
        /// Formation name
        /// </summary>
        public string Formacion { get; set; }

        /// <summary>
        /// Formation Top in fts
        /// </summary>
        public double? MdTope { get; set; }


        /// <summary>
        /// Formation top TVD in fts
        /// </summary>
        public double TvdTope { get; set; }

        /// <summary>
        /// Formation base MD in fts
        /// </summary>
        public double MdBase { get; set; }

        /// <summary>
        /// Formation base TVD in fts
        /// </summary>
        public double TvdBase { get; set; }

        public double? GradienteFormacion { get; set; }

        /// <summary>
        /// Formation fracture gradient in psi/ft
        /// </summary>
        public double? GradienteFractura { get; set; }
    }
}