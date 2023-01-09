using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace WellIntegrityCalculations.Models
{
    public class WellPressureCalculationRequestDTO
    {
        public IEnumerable<Tubular> tubulares { get; set; }
        public IEnumerable<Annulus> anulares { get; set; }
        public IEnumerable<Wellhead> cabezales { get; set; }
        public IEnumerable<Formation> formaciones { get; set; }
        public IEnumerable<Formation>? formacion_exps { get; set; }
        public IEnumerable<Accessory> accesorios { get; set; }
        public IEnumerable<LinerHanger> Liner_Hanger { get; set; }
        public IEnumerable<Test> FITLOT { get; set; }
        public Dictionary<string, double> SecurityFactors { get; set; }
        public IEnumerable<SurveyStation> Survey { get; set; }
        public DatumData ReferenceDepths { get; set; }

        public IEnumerable<WellboreGradient> PorePressureGradient { get; set; }
        public IEnumerable<WellboreGradient> FracturePressureGradient { get; set; }
        public IEnumerable<WellboreGradient> TemperatureGradient { get; set; }

    }
}
