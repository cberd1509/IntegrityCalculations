using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WellIntegrityCalculations.Models
{
    public class MawopCalculationRequestDTO
    {
        public string? WellName { get; set; }
        public string? WellboreId { get; set; }
    }
}
