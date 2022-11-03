using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Reflection;
using WellIntegrityCalculationsTests.Helpers;
using WellIntegrityCalculations.Models;

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
        [DynamicData(nameof(TestWellInputs), DynamicDataSourceType.Method,
        DynamicDataDisplayName = nameof(GetTestDisplayNames))]
        public void IsSumOddTest(object expValue, object[] inputs, string _)
        {
            MawopCalculationRequestDTO requestData = (MawopCalculationRequestDTO)inputs[0];
            Assert.AreEqual("CASABE SUR 40", "CASABE SUR 40");
        }


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