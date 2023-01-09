namespace WellIntegrityCalculations.Models
{
    public class Tubular
    {
        public string? AssemblyName { get; set; }
        public string? Descripcion { get; set; }
        public string? Liner { get; set; }
        public double Profundidad { get; set; }
        public double ProfundidadTVD { get; set; }
        public double Colapso { get; set; }
        public double Yield { get; set; }
        public double? TopeDeCemento { get; set; }
        public double? TocTVD { get; set; }
        public double? TopeDeCasing { get; set; }
        public string? cement_eval { get; set; }
        public string? SectType { get; set; }
        public double? Diameter { get; set; }
    }
}