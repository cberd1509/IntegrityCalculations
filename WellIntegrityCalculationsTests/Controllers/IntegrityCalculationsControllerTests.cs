using Microsoft.VisualStudio.TestTools.UnitTesting;
using WellIntegrityCalculations.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WellIntegrityCalculations.Services;
using NSubstitute;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using WellIntegrityCalculationsTests.Helpers;
using WellIntegrityCalculations.Models;
using WellIntegrityCalculations.Core;

namespace WellIntegrityCalculations.Controllers.Tests
{
    [TestClass()]
    public class IntegrityCalculationsControllerTests
    {
        IntegrityCalculationsController integrityCalculationController;
        public IntegrityCalculationsControllerTests() { 

            ILogger<CalculationService> calculationServiceLogger = Substitute.For<ILogger<CalculationService>>();
            IMawopDataProvider mawopDataProvider = new MawopDataProvider(Substitute.For<ILogger<IMawopDataProvider>>());
            IMaaspDataProvider maaspDataProvider = new MaaspDataProvider(Substitute.For<ILogger<IMaaspDataProvider>>());
            CalculationRulesProvider calculationRulesProvider = new CalculationRulesProvider(Substitute.For<ILogger<ICalculationRulesProvider>>());

            ICalculationService calculationService = new CalculationService(calculationServiceLogger, mawopDataProvider, calculationRulesProvider, maaspDataProvider);

            ILogger<IntegrityCalculationsController> _logger = Substitute.For<ILogger<IntegrityCalculationsController>>();
            integrityCalculationController = new IntegrityCalculationsController(_logger, calculationService);
        }

        [DataTestMethod]
        [DataRow("CASABE_SUR_40.json",DisplayName = "CASABE SUR 40 Returns Results")]
        [DataRow("CHICHIMENE_178R.json", DisplayName = "CHICHIMENE 178R Returns Results")]
        [DataRow("NAFTA_1.json", DisplayName = "NAFTA 1 Returns Results")]
        [DataRow("KINACU_1.json", DisplayName = "KINACU 1 Returns Results")]
        [DataRow("AKACIAS_77.json", DisplayName = "AKACIAS 77 Returns Results")]
        [DataRow("PAUTO_SUR_CP_11.json", DisplayName = "PAUTO SUR CP 12 Returns Results")]
        [DataRow("LIRIA_YW_12.json", DisplayName = "LIRIA YW-12 Returns Results")]
        public void CasabeSur40Test(string filename)
        {
            var well = JSONDataSourceHelper.LoadJSONToObject<WellPressureCalculationRequestDTO>(filename);
            var results = integrityCalculationController.GetMawop(well);
            Assert.IsNotNull(results);
        }

    }
}