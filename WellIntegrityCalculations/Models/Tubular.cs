namespace WellIntegrityCalculations.Models
{
    public class Tubular
    {

        /// <summary>
        /// Name of the assembly in which tubular component appears
        /// </summary>
        public string? AssemblyName { get; set; }
        
        /// <summary>
        /// Indicates wether the element belongs to a liner or not
        /// </summary>
        public string? Liner { get; set; }
        
        /// <summary>
        /// Depth in MD of component
        /// </summary>
        public double Profundidad { get; set; }
        
        /// <summary>
        /// Depth in TVD of component
        /// </summary>
        public double ProfundidadTVD { get; set; }
        /// <summary>
        /// Collapse perssure in psi for component
        /// </summary>
        public double Colapso { get; set; }
        
        /// <summary>
        /// Yield pressure in psi for component
        /// </summary>
        public double Yield { get; set; }
        
        /// <summary>
        /// Top of cement in MD. If no cement, then null
        /// </summary>
        public double? TopeDeCemento { get; set; }

        /// <summary>
        /// Top of cement in TVD. If no cement, then null
        /// </summary>
        public double? TocTVD { get; set; }

        /// <summary>
        /// Top of tubular in MD. If no cement, then null
        /// </summary>
        public double? TopeDeCasing { get; set; }

        /// <summary>
        /// Section Type Code
        /// </summary>
        public string? SectType { get; set; }

        /// <summary>
        /// Element diamater
        /// </summary>
        public double? Diameter { get; set; }
    }
}