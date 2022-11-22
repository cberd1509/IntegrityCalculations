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
        public CalculationElement GetInnerWeakestElementInAnnulus(List<Annulus> annulusList, List<AnnulusPressureDensityData> annulusPressureDensity)
        {
            Annulus annulus = annulusList.ElementAt(0);

            CalculationElement rule1Element = new CalculationElement
            {
                RuleCode = CalculationRulesCode.InnermostCasingOrTubing,
                RuleTitle = "Innermost Casing or Tubing (Annulus A)"
            };
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
                rule1Element.PressureGradient = 0.052 * annulusPressureDensity.ElementAt(0).Density;
            }
            return rule1Element;
        }


        //Rule #2: Casing Analysis for each Annulus
        public List<CalculationElement> GetExternalCasingAnalysis(
            List<Annulus> annulusList,
            List<AnnulusPressureDensityData> annulusPressureDensity,
            List<DepthGradient> fracturePressureGradient,
            List<CementJob> cementJobs
            )
        {
            List<CalculationElement> calculationElements = new List<CalculationElement>();

            for (int i = 0; i < annulusList.Count; i++)
            {
                CalculationElement ruleElement = new CalculationElement { RuleCode = CalculationRulesCode.MostExternalCasing, RuleTitle = $"Outermost Casing (Anulus {SchematicHelperFunctions.ALPHABET[i]})" };

                Annulus currentAnnulus = annulusList[i];
                CasingData element = SchematicHelperFunctions.GetOuterWeakestElementFromAnnulus(currentAnnulus);
                CasingData deepestElement = currentAnnulus.OuterBoundary.Last();

                ruleElement.IsRelevant = true; //TODO: TBD
                ruleElement.Diameter = element.Diameter;
                ruleElement.CasingShoeTvd = element.TvdBase;

                CementJob? associatedCementJob = SchematicHelperFunctions.GetAnnulusCementJob(currentAnnulus, cementJobs);
                if (associatedCementJob != null)
                {
                    ruleElement.TopOfCementInAnular = associatedCementJob.CementTop;
                }

                if (associatedCementJob != null && associatedCementJob.CementTop <= 0)
                {
                    ruleElement.PressureGradient = 0;
                }
                else
                {
                    ruleElement.PressureGradient = 0.052 * annulusPressureDensity.ElementAt(i).Density;
                }

                ruleElement.BurstPressure = element.BurstPressure;
                ruleElement.CollapsePressure = element.CollapsePressure;
                ruleElement.BelowFormationFractureGradient = SchematicHelperFunctions.GetGradientValueAtDepth(deepestElement.TvdBase, fracturePressureGradient, 0);

                calculationElements.Add(ruleElement);
            }

            return calculationElements;
        }

        //Rule #3: Subsurface Safety Valve
        public CalculationElement GetSubsurfaceSafetyValveAnalysis(List<AssemblyComponent> assemblies)
        {
            CalculationElement ruleElement = new CalculationElement { RuleCode = CalculationRulesCode.SubsurfaceSafetyValve, RuleTitle = $"Subsurface Safety Valve" };

            //TODO: Have in mind case with Multiple SSV (Change Find for FindAll)
            var subsurfaceSafetyValveData = assemblies.Find(x => x.AssemblyType == "SSV");
            
            if (subsurfaceSafetyValveData == null)
            {
                ruleElement.IsRelevant = false;
            }
            else
            {
                ruleElement.Diameter = subsurfaceSafetyValveData.Diameter;
                ruleElement.ComponentTvd = subsurfaceSafetyValveData.Tvd; //TODO: If not present, we have to interpolate it
                ruleElement.MaxOperationRatingPressure = subsurfaceSafetyValveData.MaxOperationPressure;
                ruleElement.CollapsePressure = subsurfaceSafetyValveData.CollapsePressure;
            }
            return ruleElement;
        }

        //Rule #4: BHA Lowest Rating Accessory
        public CalculationElement GetBhaLowestRatingAccessoryAnalysis(List<AssemblyComponent> assemblies)
        {
            List<AssemblyComponent> matchingElements = assemblies.FindAll(assembly =>
            {
                return assembly.AssemblyType != "SSV" && assembly.AssemblyType != "PACKER"; //TODO: Look for the Packer code
            });

            CalculationElement ruleElement = new CalculationElement { RuleCode = CalculationRulesCode.SubsurfaceSafetyValve, RuleTitle = $"Weakest BHA Element" };

            matchingElements.OrderBy(x => x.MaxOperationPressure);


            return ruleElement;
        }

        //Rule #5: Prod/Iny Top Packer 
        public CalculationElement GetTopPackerAnalysis()
        {
            throw new NotImplementedException();
        }

        //Rule #6: Top Liner Hanger Analysis
        public CalculationElement GetTopLinerHangerAnalysis()
        {
            throw new NotImplementedException();
        }

        //Rule #7: Wellhead Analysis
        public List<CalculationElement> GetWellheadAnalysis()
        {
            throw new NotImplementedException();
        }
    }
}
