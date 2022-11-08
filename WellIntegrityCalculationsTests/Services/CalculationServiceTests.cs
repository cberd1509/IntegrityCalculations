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

        #region GetAnnulusContents Tests

        [TestMethod("Tests a Well with 3 Casing Sections")]
        public void TestThreeSingleCasings()
        {
            //Arrange Test

            CasingData casing1 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 2000, mdTop: 0, diameter: 14);
            CasingData casing2 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 4000, mdTop: 0, diameter: 8.5);
            CasingData casing3 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 6000, mdTop: 0, diameter: 5.5);

            //Input Data
            List<CasingData> casingData = new List<CasingData> {
                casing1, casing3, casing2
            };

            //Expected Output Data
            List<Annulus> annulusContentExpected = new List<Annulus>
            {
                new Annulus(0, new List<CasingData>(), new List<CasingData>{ casing3 }),
                new Annulus(1, new List<CasingData>{ casing3 }, new List<CasingData>{ casing2 }),
                new Annulus(2, new List<CasingData>{ casing2 }, new List<CasingData>{ casing1 }),
            };

            //Make Test
            List<Annulus> annulusContentsResult = _calculationService.GetAnnulusContents(casingData);

            //Assert
            Assert.AreEqual(annulusContentExpected.Count, annulusContentsResult.Count, message: "Wrong Annulus Count");
            for (int i = 0; i < annulusContentsResult.Count; i++)
            {
                Assert.AreEqual(
                    JsonSerializer.Serialize(annulusContentsResult[i]),
                    JsonSerializer.Serialize(annulusContentExpected[i]),
                    message: $"Annulus {i} differs from expected"
                    );
            }
        }

        [TestMethod("Tests a Well with 3 Casing Sections and a tubing")]
        public void TestThreeSingleCasingsAndTubing()
        {
            //Arrange Test

            CasingData casing1 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 2000, mdTop: 0, diameter: 14);
            CasingData casing2 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 4000, mdTop: 0, diameter: 8.5);
            CasingData casing3 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 6000, mdTop: 0, diameter: 5.5);
            CasingData tubing = new CasingData(sectType: CasingSectionType.TUBING, mdBase: 5500, mdTop: 0, diameter: 3.5);

            //Input Data
            List<CasingData> casingData = new List<CasingData> {
                casing1, casing3, casing2, tubing
            };

            //Expected Output Data
            List<Annulus> annulusContentExpected = new List<Annulus>
            {
                new Annulus(0, new List<CasingData>{ tubing }, new List<CasingData>{ casing3 }),
                new Annulus(1, new List<CasingData>{ casing3 }, new List<CasingData>{ casing2 }),
                new Annulus(2, new List<CasingData>{ casing2 }, new List<CasingData>{ casing1 }),
            };

            //Make Test
            List<Annulus> annulusContentsResult = _calculationService.GetAnnulusContents(casingData);

            //Assert
            Assert.AreEqual(annulusContentExpected.Count, annulusContentsResult.Count, message: "Wrong Annulus Count");
            for (int i = 0; i < annulusContentsResult.Count; i++)
            {
                Assert.AreEqual(
                    JsonSerializer.Serialize(annulusContentsResult[i]),
                    JsonSerializer.Serialize(annulusContentExpected[i]),
                    message: $"Annulus {i} differs from expected"
                    );
            }
        }

        [TestMethod("Tests a Well with 3 Casing Sections and a liner")]
        public void TestThreeSingleCasingsAndLiner()
        {
            //Arrange Test

            CasingData casing1 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 2000, mdTop: 0, diameter: 14);
            CasingData casing2 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 4000, mdTop: 0, diameter: 8.5);
            CasingData casing3 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 6000, mdTop: 0, diameter: 7.5);
            CasingData liner = new CasingData(sectType: CasingSectionType.LINER, mdBase: 7000, mdTop: 5500, diameter: 5.5);

            //Input Data
            List<CasingData> casingData = new List<CasingData> {
                casing1, casing3, casing2, liner
            };

            //Expected Output Data
            List<Annulus> annulusContentExpected = new List<Annulus>
            {
                new Annulus(0, new List<CasingData>(), new List<CasingData>{ liner, casing3}),
                new Annulus(1, new List<CasingData>{ liner,casing3 }, new List<CasingData>{ casing2 }),
                new Annulus(2, new List<CasingData>{ casing2 }, new List<CasingData>{ casing1 }),
            };

            //Make Test
            List<Annulus> annulusContentsResult = _calculationService.GetAnnulusContents(casingData);

            //Assert
            Assert.AreEqual(annulusContentExpected.Count, annulusContentsResult.Count, message: "Wrong Annulus Count");
            for (int i = 0; i < annulusContentsResult.Count; i++)
            {
                Assert.AreEqual(
                    JsonSerializer.Serialize(annulusContentsResult[i]),
                    JsonSerializer.Serialize(annulusContentExpected[i]),
                    message: $"Annulus {i} differs from expected"
                    );
            }
        }

        [TestMethod("Tests a Well with 3 Casing Sections and a liner and a tubing")]
        public void TestThreeSingleCasingsAndLinerAndTubing()
        {
            //Arrange Test

            CasingData casing1 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 2000, mdTop: 0, diameter: 14);
            CasingData casing2 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 4000, mdTop: 0, diameter: 8.5);
            CasingData casing3 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 6000, mdTop: 0, diameter: 7.5);
            CasingData liner = new CasingData(sectType: CasingSectionType.LINER, mdBase: 7000, mdTop: 5500, diameter: 5.5);
            CasingData tubing = new CasingData(sectType: CasingSectionType.TUBING, mdBase: 7000, mdTop: 0, diameter: 3.5);

            //Input Data
            List<CasingData> casingData = new List<CasingData> {
                casing1, casing3, casing2, liner, tubing
            };

            //Expected Output Data
            List<Annulus> annulusContentExpected = new List<Annulus>
            {
                new Annulus(0, new List<CasingData>{ tubing }, new List<CasingData>{ liner, casing3 }),
                new Annulus(1, new List<CasingData>{ liner,casing3 }, new List<CasingData>{ casing2 }),
                new Annulus(2, new List<CasingData>{ casing2 }, new List<CasingData>{ casing1 }),
            };

            //Make Test
            List<Annulus> annulusContentsResult = _calculationService.GetAnnulusContents(casingData);

            //Assert
            Assert.AreEqual(annulusContentsResult.Count, annulusContentExpected.Count, message: "Wrong Annulus Count");
            for (int i = 0; i < annulusContentsResult.Count; i++)
            {
                Assert.AreEqual(
                    JsonSerializer.Serialize(annulusContentsResult[i]),
                    JsonSerializer.Serialize(annulusContentExpected[i]),
                    message: $"Annulus {i} differs from expected"
                    );
            }
        }

        [TestMethod("Tests a Well with 3 Casing Sections and two liners and a tubing")]
        public void TestThreeSingleCasingsAndTwoLinerAndTubing()
        {
            //Arrange Test

            CasingData casing1 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 2000, mdTop: 0, diameter: 18);
            CasingData casing2 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 4000, mdTop: 0, diameter: 13.75);
            CasingData casing3 = new CasingData(sectType: CasingSectionType.CASING, mdBase: 6000, mdTop: 0, diameter: 9.5);
            CasingData liner1 = new CasingData(sectType: CasingSectionType.LINER, mdBase: 7000, mdTop: 5500, diameter: 7.5);
            CasingData liner2 = new CasingData(sectType: CasingSectionType.LINER, mdBase: 10000, mdTop: 6500, diameter: 5.5);
            CasingData tubing = new CasingData(sectType: CasingSectionType.TUBING, mdBase: 9000, mdTop: 0, diameter: 3.5);

            //Input Data
            List<CasingData> casingData = new List<CasingData> {
                casing1, casing3, casing2, liner1, liner2, tubing
            };

            //Expected Output Data
            List<Annulus> annulusContentExpected = new List<Annulus>
            {
                new Annulus(0, new List<CasingData>{ tubing }, new List<CasingData>{ liner2, liner1, casing3 }),
                new Annulus(1, new List<CasingData>{ liner2, liner1, casing3 }, new List<CasingData>{ casing2 }),
                new Annulus(2, new List<CasingData>{ casing2 }, new List<CasingData>{ casing1 }),
            };

            //Make Test
            List<Annulus> annulusContentsResult = _calculationService.GetAnnulusContents(casingData);

            //Assert
            Assert.AreEqual(annulusContentsResult.Count, annulusContentExpected.Count, message: "Wrong Annulus Count");
            for (int i = 0; i < annulusContentsResult.Count; i++)
            {
                Assert.AreEqual(
                    JsonSerializer.Serialize(annulusContentsResult[i]),
                    JsonSerializer.Serialize(annulusContentExpected[i]),
                    message: $"Annulus {i} differs from expected"
                    );
            }
        }

        #endregion

        #region Full Tests

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

        [TestMethod()]
        public void GetInnerWeakestElementFromAnnulusTest()
        {
            //Arrange Test
            MawopCalculationRequestDTO requestData = JSONDataSourceHelper.LoadJSONToObject<MawopCalculationRequestDTO>("SampleTestWell.json");

            //Make Test
            var annulusData = _calculationService.GetAnnulusContents(requestData.CasingData);
            Assert.IsNotNull(annulusData);

            var element = _calculationService.GetInnerWeakestElementFromAnnulus(annulusData[0]);

            Assert.AreEqual(3.5, element.Diameter);
            
        }

        [TestMethod()]
        public void GetOuterWeakestElementFromAnnulusTest()
        {
            //Arrange Test
            MawopCalculationRequestDTO requestData = JSONDataSourceHelper.LoadJSONToObject<MawopCalculationRequestDTO>("SampleTestWell.json");

            //Make Test
            var annulusData = _calculationService.GetAnnulusContents(requestData.CasingData);
            Assert.IsNotNull(annulusData);

            var element = _calculationService.GetOuterWeakestElementFromAnnulus(annulusData[0]);

            Assert.AreEqual(9.75, element.Diameter);

        }
    }
}