namespace WellIntegrityCalculations.Models
{
    public class Accessory
    {
        public string? Accesorio { get; set; }
        public double Profundidad { get; set; }
        public string? Tipo { get; set; }
        public double RatingDePresion { get; set; }
        public string? AssemblyAlQuePertenece { get; set; }
        public double CollapsePressure { get; set; }
        public string? CompType { get; set; }
    }
}