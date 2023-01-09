namespace WellIntegrityCalculations.Models
{
    public class Formation
    {
        public string? Formacion { get; set; }
        public double? MdTope { get; set; }
        public double? TvdTope { get; set; }

        public double? MdBase { get; set; }

        public double? TvdBase { get; set; }
        public double? GradienteFormacion { get; set; }
        public double? GradienteFractura { get; set; }
    }
}