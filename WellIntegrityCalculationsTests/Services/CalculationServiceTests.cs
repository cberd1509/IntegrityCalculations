using WellIntegrityCalculations.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Reflection;
using WellIntegrityCalculationsTests.Helpers;
using WellIntegrityCalculations.Models;
using System.Text.Json;

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

        #region Full Tests

        [TestMethod("Full Test - Well 1 - Test Data Loads Correctly")]
        public void TestWell1DataLoad()
        {
            //Arrange Test
            MawopCalculationRequestDTO requestData = JSONDataSourceHelper.LoadJSONToObject<MawopCalculationRequestDTO>("SampleTestWell.json");

            //Make Test

            //Test Casing Data
            Assert.AreEqual(6, requestData.CasingData.Count);
            
            Assert.AreEqual(3.5, requestData.CasingData.Last().Diameter);
            Assert.AreEqual(CasingSectionType.TUBING, requestData.CasingData.Last().SectType);

            //Test Gradient Data
            Assert.AreEqual(4, requestData.TemperatureGradient.Count);
            Assert.AreEqual(220, requestData.TemperatureGradient.Last().Value);

            Assert.AreEqual(3, requestData.PorePressureGradient.Count);
            Assert.AreEqual(8000, requestData.PorePressureGradient.Last().Value);

            Assert.AreEqual(3, requestData.FracturePressureGradient.Count);
            Assert.AreEqual(27000, requestData.FracturePressureGradient.Last().Value);

        }

        [TestMethod("Full Test - Well 1")]
        public void GetWellMawop()
        {
            //Arrange Test
            MawopCalculationRequestDTO requestData = JSONDataSourceHelper.LoadJSONToObject<MawopCalculationRequestDTO>("SampleTestWell.json");

            //Make Test
            _calculationService.GetWellMawop(requestData);
        }

        #endregion

        #region Helper functions for Test Creation
        public static IEnumerable<object[]> TestWellInputs()
        {
            return new[]
            {
                new object[] { true, new object[] { JSONDataSourceHelper.LoadJSONToObject<MawopCalculationRequestDTO>("CasabeSur40.json") }, "CASABE SUR 40" },
                new object[] { true, new object[] { JSONDataSourceHelper.LoadJSONToObject<MawopCalculationRequestDTO>("Nafta40.json") }, "NAFTA 40" }
            };
        }

        public static string GetTestDisplayNames(MethodInfo methodInfo, object[] values)
        {
            var expected = (bool)values[0];
            var name = (string)values[2];
            return $"{methodInfo.Name}({expected}, {name})";
        }
        #endregion

    }
}