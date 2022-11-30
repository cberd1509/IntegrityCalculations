using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace WellIntegrityCalculations.Models
{
    public class MawopCalculationRequestDTO
    {
        public IEnumerable<Tubular> tubulares { get; set; }
        public IEnumerable<Annulus> anulares { get; set; }
        public IEnumerable<Wellhead> cabezales{ get; set; }
        public IEnumerable<Formation> formaciones{ get; set; }
        public IEnumerable<Formation>? formacion_exps{ get; set; }
        public IEnumerable<Accessory> accesorios{ get; set; }
        public IEnumerable<LinerHanger> Liner_Hanger{ get; set; }
        public IEnumerable<Test> FITLOT { get; set; }
        public double BurstSecurityFactor { get; set; }
        public double CollapseSecurityFactor { get; set; }
        public double Datum { get; set; }

    }
}
