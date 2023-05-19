using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;
using WellIntegrityCalculations.Models;
using WellIntegrityCalculations.RequestExamples;
using WellIntegrityCalculations.Services;

namespace WellIntegrityCalculations.Controllers
{
    /// <summary>
    /// Contains rutes related to Integrity Calculations
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class IntegrityCalculationsController : ControllerBase
    {
        private readonly ILogger<IntegrityCalculationsController> _logger;
        private readonly ICalculationService _calculationService;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="calculationService"></param>
        public IntegrityCalculationsController(ILogger<IntegrityCalculationsController> logger, ICalculationService calculationService)
        {
            _logger = logger;
            _calculationService = calculationService;
        }

        /// <summary>
        /// Performs operative limits calculations for a given well data
        /// </summary>
        /// <param name="requestData">Payload containing well data</param>
        /// <returns>Calculation Results for MAASP and MAWOP</returns>
        /// <response code="200">Returns the calculations results</response>
        /// <response code="400">If the information sent is invalid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [SwaggerRequestExample(typeof(WellPressureCalculationRequestDTO), typeof(WellPressureCalculationRequestDTOExample))]
        public GenericAPIResponseDTO<List<WellPressureCalculationResult>> RunCalculations(WellPressureCalculationRequestDTO requestData)
        {
            _logger.LogInformation($"Requesting MAWOP Calculations");
            _logger.LogInformation($"JSON DATA: {JsonSerializer.Serialize(requestData).ToString()}");
            GenericAPIResponseDTO<List<WellPressureCalculationResult>> response = _calculationService.GetWellMawop(requestData);
            _logger.LogInformation("MAWOP Calculations were successful, sending response", response);
            return response;

        }
    }
}
