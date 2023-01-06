using Microsoft.VisualStudio.TestTools.UnitTesting;
using WellIntegrityCalculations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WellIntegrityCalculations.Models;
using System.Text.Json;
using WellIntegrityCalculationsTests.Helpers;

namespace WellIntegrityCalculations.Core.Tests
{
    [TestClass()]
    public class SchematicHelperFunctionsTests
    {
        [TestMethod()]
        public void GetInnerWeakestElementFromAnnulusTest()
        {
            //Arrange Test
            WellPressureCalculationRequestDTO requestData = JSONDataSourceHelper.LoadJSONToObject<WellPressureCalculationRequestDTO>("SampleTestWell.json");

            //Make Test
            var annulusData = SchematicHelperFunctions.GetAnnulusContents(requestData.CasingData);
            Assert.IsNotNull(annulusData);

            var element = SchematicHelperFunctions.GetInnerWeakestElementFromAnnulus(annulusData[0]);

            Assert.AreEqual(3.5, element.Diameter);

        }

        [TestMethod()]
        public void GetOuterWeakestElementFromAnnulusTest()
        {
            //Arrange Test
            WellPressureCalculationRequestDTO requestData = JSONDataSourceHelper.LoadJSONToObject<WellPressureCalculationRequestDTO>("SampleTestWell.json");

            //Make Test
            var annulusData = SchematicHelperFunctions.GetAnnulusContents(requestData.CasingData);
            Assert.IsNotNull(annulusData);

            var element = SchematicHelperFunctions.GetOuterWeakestElementFromAnnulus(annulusData[0]);

            Assert.AreEqual(9.75, element.Diameter);

        }


        #region GetGradientAtDepthTests

        [TestMethod("Get Gradient when Gradient Map is empty")]
        [TestCategory("Get Gradient at Depth Tests")]
        public void GetGradientValueOnEmptyGradient()
        {
            //Arrange
            var gradient = new List<DepthGradient>();

            //Test
            Assert.ThrowsException<Exception>(()=>SchematicHelperFunctions.GetGradientValueAtDepth(1000, gradient, 60));
        }

        [TestMethod("Get Gradient for a depth between gradient values")]
        [TestCategory("Get Gradient at Depth Tests")]
        public void GetGradientValueOnDepthBetweenGradientValues()
        {
            //Arrange
            var gradient = new List<DepthGradient> {
                new DepthGradient{ Depth = 1000, Value = 500},
                new DepthGradient{ Depth = 2000, Value = 1000},
                new DepthGradient{ Depth = 3000, Value = 3000},
                new DepthGradient{ Depth = 4000, Value = 2000},
                new DepthGradient{ Depth = 5000, Value = 2500},
                new DepthGradient{ Depth = 6000, Value = 3000},
            };

            Assert.AreEqual(0.5, SchematicHelperFunctions.GetGradientValueAtDepth(1500, gradient, 0));
            Assert.AreEqual(2, SchematicHelperFunctions.GetGradientValueAtDepth(2500, gradient, 0));
        }

        [TestMethod("Get Gradient for a depth above first value of the gradient map")]
        [TestCategory("Get Gradient at Depth Tests")]
        public void GetGradientValueOnDeptAboveFirstValue()
        {
            //Arrange
            var gradient = new List<DepthGradient> {
                new DepthGradient{ Depth = 1000, Value = 500},
                new DepthGradient{ Depth = 2000, Value = 1000},
                new DepthGradient{ Depth = 3000, Value = 3000},
                new DepthGradient{ Depth = 4000, Value = 2000},
                new DepthGradient{ Depth = 5000, Value = 2500},
                new DepthGradient{ Depth = 6000, Value = 3000},
            };

            Assert.AreEqual(0.5, SchematicHelperFunctions.GetGradientValueAtDepth(50, gradient, 0));
        }

        [TestMethod("Get Gradient for a depth below last value of gradient map")]
        [TestCategory("Get Gradient at Depth Tests")]
        public void GetGradientValueOnDeptBelowLastValue()
        {
            //Arrange
            var gradient = new List<DepthGradient> {
                new DepthGradient{ Depth = 1000, Value = 500},
                new DepthGradient{ Depth = 2000, Value = 1000},
                new DepthGradient{ Depth = 3000, Value = 3000},
                new DepthGradient{ Depth = 4000, Value = 2000},
                new DepthGradient{ Depth = 5000, Value = 2000},
                new DepthGradient{ Depth = 6000, Value = 3000},
            };

            Assert.AreEqual(1, SchematicHelperFunctions.GetGradientValueAtDepth(7000, gradient, 0));
        }

        #endregion

        #region GetAnnulusContents Tests

        [TestMethod("Tests a Well with 3 Casing Sections")]
        [TestCategory("Get Annulus Contents")]
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
            List<Annulus> annulusContentsResult = SchematicHelperFunctions.GetAnnulusContents(casingData);

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
        [TestCategory("Get Annulus Contents")]
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
            List<Annulus> annulusContentsResult = SchematicHelperFunctions.GetAnnulusContents(casingData);

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
        [TestCategory("Get Annulus Contents")]
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
            List<Annulus> annulusContentsResult = SchematicHelperFunctions.GetAnnulusContents(casingData);

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
        [TestCategory("Get Annulus Contents")]
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
            List<Annulus> annulusContentsResult = SchematicHelperFunctions.GetAnnulusContents(casingData);

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
        [TestCategory("Get Annulus Contents")]
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
            List<Annulus> annulusContentsResult = SchematicHelperFunctions.GetAnnulusContents(casingData);

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

    }
}