using Microsoft.VisualStudio.TestTools.UnitTesting;
using WellIntegrityCalculations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WellIntegrityCalculations.Services;
using WellIntegrityCalculations.Models;
using WellIntegrityCalculationsTests.Helpers;
using Microsoft.VisualStudio.TestPlatform.Common;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

namespace WellIntegrityCalculations.Core.Tests
{
    [TestClass()]
    public class CalculationRulesProviderTests
    {

        private readonly ICalculationRulesProvider _calculationRulesProvider;

        public CalculationRulesProviderTests()
        {
            ILogger<ICalculationService> _logger = Substitute.For<ILogger<ICalculationService>>();
            _calculationRulesProvider = new CalculationRulesProvider(_logger);
        }

        [TestMethod("Get Inner Weakest Element for Sample Test Well 1")]
        [TestCategory("Rule 1 Tests: Innermost Weakest Element")]
        public void GetInnerWeakestElementInAnnulusTestTestWell()
        {
            //Prepare Test
            const string testFilename = "SampleTestWell.json";
            MawopCalculationRequestDTO requestData = JSONDataSourceHelper.LoadJSONToObject<MawopCalculationRequestDTO>(testFilename);

            List<Annulus> annulusList = SchematicHelperFunctions.GetAnnulusContents(requestData.CasingData);
            List<DepthGradient> porePressureGradient = requestData.PorePressureGradient;

            Assert.IsNotNull(annulusList);
            Assert.IsNotNull(porePressureGradient);

            //Runs Function
            var res = _calculationRulesProvider.GetInnerWeakestElementInAnnulus(annulusList, porePressureGradient);
            Assert.AreEqual(3.5, res.Diameter);
            Assert.AreEqual(0.5, res.PressureGradient);
            Assert.AreEqual(true, res.IsRelevant);
        }

        [TestMethod("Get Inner Weakest Element for Sample Test Well 1 - No Tubing")]
        [TestCategory("Rule 1 Tests: Innermost Weakest Element")]
        public void GetInnerWeakestElementInAnnulusTestTestWellWithoutTubing()
        {
            //Prepare Test

            const string testFilename = "SampleTestWell.json"; 
            MawopCalculationRequestDTO requestData = JSONDataSourceHelper.LoadJSONToObject<MawopCalculationRequestDTO>(testFilename);

            List<Annulus> annulusList = SchematicHelperFunctions.GetAnnulusContents(requestData.CasingData);
            List<DepthGradient> porePressureGradient = requestData.PorePressureGradient;

            Assert.IsNotNull(porePressureGradient);

            //Checks that there's a tubing
            Assert.AreEqual(1, annulusList.ElementAt(0).InnerBoundary.Count);
            //Removes it
            annulusList.ElementAt(0).InnerBoundary.Clear();

            //Runs Function
            var res = _calculationRulesProvider.GetInnerWeakestElementInAnnulus(annulusList, porePressureGradient);
            Assert.AreEqual(false, res.IsRelevant);
        }

        [TestMethod("Get Most External Casings for Test Well 1")]
        [TestCategory("Rule 2 Tests: Outermost Weakest Element for Annulus")]
        public void GetExternalCasingAnalysisTestTestWell()
        {
            //Prepare Test
            const string testFilename = "SampleTestWell.json";
            MawopCalculationRequestDTO requestData = JSONDataSourceHelper.LoadJSONToObject<MawopCalculationRequestDTO>(testFilename);

            List<Annulus> annulusList = SchematicHelperFunctions.GetAnnulusContents(requestData.CasingData);
            List<DepthGradient> porePressureGradient = requestData.PorePressureGradient;
            List<DepthGradient> fracturePressureGradient = requestData.FracturePressureGradient;

            Assert.IsNotNull(annulusList);
            Assert.IsNotNull(porePressureGradient);
            Assert.IsNotNull(fracturePressureGradient);

            //Runs Function
            var res = _calculationRulesProvider.GetExternalCasingAnalysis(annulusList, porePressureGradient, fracturePressureGradient);
            Assert.AreEqual(3, res.Count);
            Assert.AreEqual(9.75, res.ElementAt(0).Diameter);
            Assert.AreEqual(true, res.ElementAt(0).IsRelevant);
        }

    }
}