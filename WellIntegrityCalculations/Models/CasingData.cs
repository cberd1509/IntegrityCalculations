namespace WellIntegrityCalculations.Models
{
    public class CasingData
    {
        public CasingSectionType SectType { get; set; }
        public double MdTop { get; set;  }
        public double MdBase { get; set; }
        public double Diameter { get; set; }
        public double BurstPressure { get; set; }
        public double CollapsePressure { get; set; }
        public double TvdBase { get; set; }

        public CasingData(CasingSectionType sectType, double mdTop, double mdBase, double diameter)
        {
            SectType = sectType;
            MdTop = mdTop;
            MdBase = mdBase;
            Diameter = diameter;
        }
    }
}
