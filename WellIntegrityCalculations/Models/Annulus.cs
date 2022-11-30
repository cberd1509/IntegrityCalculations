namespace WellIntegrityCalculations.Models
{
    public class Annulus
    {
       public int? index { get; set; }
        public string? Anular { get; set; }
        public double? Densidad { get; set; }
        public double? Presion { get; set; }

        public IList<Tubular>? InnerBoundary { get; set; }
        public IList<Tubular>? OuterBoundary { get; set; }
    }
}
