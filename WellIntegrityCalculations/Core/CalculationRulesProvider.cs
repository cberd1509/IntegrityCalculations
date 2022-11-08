using System.Xml.Linq;
using WellIntegrityCalculations.Models;
using WellIntegrityCalculations.Services;

namespace WellIntegrityCalculations.Core
{
    public class CalculationRulesProvider : ICalculationRulesProvider
    {
        ILogger<ICalculationService> _logger;

        public CalculationRulesProvider(ILogger<ICalculationService> logger)
        {
            _logger = logger;
        }

        //Rule #1: Inner Weakest Element in Annulus
        public CalculationElement GetInnerWeakestElementInAnnulus(List<Annulus> annulusList, List<DepthGradient> porePressureGradient)
        {
            Annulus annulus = annulusList.ElementAt(0);

            CalculationElement rule1Element = new CalculationElement { RuleCode = CalculationRulesCode.InnermostCasingOrTubing, RuleTitle = "Innermost Casing or Tubing (Annulus A)" };
            if (annulus.InnerBoundary.Count == 0)
            {
                rule1Element.IsRelevant = false;
            }
            else
            {
                CasingData element = SchematicHelperFunctions.GetInnerWeakestElementFromAnnulus(annulus);
                rule1Element.IsRelevant = true;
                rule1Element.Diameter = element.Diameter;
                rule1Element.CollapsePressure = element.CollapsePressure;
                rule1Element.BurstPressure = element.BurstPressure;
                rule1Element.PressureGradient = SchematicHelperFunctions.GetGradientValueAtDepth(element.TvdBase, porePressureGradient, 0);
            }
            return rule1Element;
        }


        //Rule #2: Casing Analysis for each Annulus
        public List<CalculationElement> GetExternalCasingAnalysis(List<Annulus> annulusList, List<DepthGradient> porePressureGradient, List<DepthGradient> fracturePressureGradient)
        {
            List<CalculationElement> calculationElements = new List<CalculationElement>();

            for (int i = 0; i < annulusList.Count; i++)
            {
                CalculationElement ruleElement = new CalculationElement { RuleCode = CalculationRulesCode.MostExternalCasing, RuleTitle = $"Outermost Casing (Anulus {SchematicHelperFunctions.ALPHABET[i]})" };
                CasingData element = SchematicHelperFunctions.GetOuterWeakestElementFromAnnulus(annulusList[i]);

                ruleElement.IsRelevant = true;
                ruleElement.Diameter = element.Diameter;
                ruleElement.CasingShoeTvd = element.TvdBase;

                if (true == false) //TODO: Implement this
                {
                    ruleElement.TopOfCementInAnular = 0;
                }

                ruleElement.BurstPressure = element.BurstPressure;
                ruleElement.CollapsePressure = element.CollapsePressure;
                ruleElement.PressureGradient = SchematicHelperFunctions.GetGradientValueAtDepth(element.TvdBase, porePressureGradient, 0);
                ruleElement.BelowFormationFractureGradient = SchematicHelperFunctions.GetGradientValueAtDepth(element.TvdBase, fracturePressureGradient, 0); //TODO: Is this correct ?

                calculationElements.Add(ruleElement);
            }

            return calculationElements;
        }
    
        //Rule #3: Subsurface Safety Valve
        public CalculationElement GetSubsurfaceSafetyValveAnalysis()
        {
            throw new NotImplementedException();
        }

        //Rule #4: BHA Lowest Rating Accessory
        public CalculationElement GetBhaLowestRatingAccessoryAnalysis()
        {
            throw new NotImplementedException();
        }

        //Rule #5: Prod/Iny Top Packer 
        public CalculationElement GetTopPackerAnalysis()
        {
            throw new NotImplementedException();
        }

        //Rule #6: Top Liner Hanger Analysis
        public CalculationElement GetTopLinerHangerAnalysis (){
            throw new NotImplementedException();
        }

        //Rule #7: Wellhead Analysis
        public List<CalculationElement> GetWellheadAnalysis()
        {
            throw new NotImplementedException();
        }
    }
}
