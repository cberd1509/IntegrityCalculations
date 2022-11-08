using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WellIntegrityCalculations.Models
{
    public class MawopCalculationRequestDTO
    {
        public bool IsOffshore { get; set; }
        public List<CasingData> CasingData { get; set; }
    }
}
