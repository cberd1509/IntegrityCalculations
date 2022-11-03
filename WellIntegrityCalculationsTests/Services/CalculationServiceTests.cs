using Microsoft.VisualStudio.TestTools.UnitTesting;
using WellIntegrityCalculations.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace WellIntegrityCalculations.Services.Tests
{
    [TestClass()]
    public class CalculationServiceTests
    {
        private readonly ICalculationService _calculationService;

        public CalculationServiceTests()
        {
            ILogger<ICalculationService> _logger = Substitute.For<ILogger<ICalculationService>>();
            _calculationService = new CalculationService(_logger);
        }

        [TestMethod()]
        [DataRow(1, 1, DisplayName = "With 1+1")]
        [DataRow(1, 2, DisplayName = "With 1+2")]
        [DataRow(1, 3, DisplayName = "With 1+3")]
        [DataRow(1, 4, DisplayName = "With 1+4")]
        public void IsSumOddTest(int x, int y)
        {
            Assert.AreEqual(true, _calculationService.IsSumOdd(x, y));
        }
    }
}