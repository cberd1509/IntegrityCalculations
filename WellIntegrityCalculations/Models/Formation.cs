using System.Text.Json.Serialization;
using WellIntegrityCalculations.Core;

namespace WellIntegrityCalculations.Models
{
    public class Formation
    {
        public string? Formacion { get; set; }
        public double? MdTope { get; set; }
        public double? TvdTope { get; set; }

        public double? MdBase { get; set; }

        public double? TvdBase { get; set; }
        
        public object? GradienteFormacion { get; set; }
        
        public object? GradienteFractura { get; set; }
    }
}