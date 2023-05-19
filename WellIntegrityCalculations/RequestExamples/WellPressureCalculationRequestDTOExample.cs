using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;
using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.RequestExamples
{
    public class WellPressureCalculationRequestDTOExample : IExamplesProvider<WellPressureCalculationRequestDTO>
    {
        public WellPressureCalculationRequestDTO GetExamples()
        {
            try
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"RequestExamples\\CASABE_SUR_40.json");
                var json = File.ReadAllText(filePath);

                WellPressureCalculationRequestDTO deserializedObj = JsonSerializer.Deserialize<WellPressureCalculationRequestDTO>(json)!;
                return deserializedObj!;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
