using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WellIntegrityCalculations.Models;
using WellIntegrityCalculations.Services;

namespace WellIntegrityCalculations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrityCalculationsController : ControllerBase
    {
        private readonly ILogger<IntegrityCalculationsController> _logger;
        private readonly ICalculationService _calculationService;
        public IntegrityCalculationsController(ILogger<IntegrityCalculationsController> logger, ICalculationService calculationService)
        {
            _logger = logger;
            _calculationService = calculationService;
        }
        
       /// <summary>
       /// 
       /// </summary>
       /// <param name="requestData"></param>
       /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public GenericAPIResponseDTO GetMawop(MawopCalculationRequestDTO requestData)
        {
            return _calculationService.GetWellMawop();
        }
    }
}
