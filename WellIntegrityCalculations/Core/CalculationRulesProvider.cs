﻿using System.Xml.Linq;
using WellIntegrityCalculations.Models;
using WellIntegrityCalculations.Services;

namespace WellIntegrityCalculations.Core
{
    public class CalculationRulesProvider : ICalculationRulesProvider
    {
        ILogger<ICalculationRulesProvider> _logger;

        public CalculationRulesProvider(ILogger<ICalculationRulesProvider> logger)
        {
            _logger = logger;
        }


        public List<CalculationElement> GetCalculationElements(WellPressureCalculationRequestDTO data)
        {
            List<CalculationElement> calculationElements = new List<CalculationElement>();

            calculationElements.Add(GetInnerWeakestElementInAnnulus(data));
            calculationElements.AddRange(GetExternalCasingAnalysis(data));
            calculationElements.Add(GetSubsurfaceSafetyValveAnalysis(data));
            calculationElements.Add(GetBhaLowestRatingAccessoryAnalysis(data));
            calculationElements.Add(GetTopPackerAnalysis(data));
            calculationElements.Add(GetTopLinerHangerAnalysis(data));
            calculationElements.AddRange(GetWellheadAnalysis(data));

            return calculationElements;
        }

        //Rule #1: Inner Weakest Element in Annulus
        CalculationElement GetInnerWeakestElementInAnnulus(WellPressureCalculationRequestDTO data)
        {
            List<Annulus> annulusWithContentsList = SchematicHelperFunctions.GetAnnulusContents(data.tubulares).ToList();

            if (annulusWithContentsList.Count != data.anulares.ToList().Count - 1)
            {
                //TODO: Revisar si se debe lanzar una excepcion
            }


            Tubular weakest = annulusWithContentsList[0].InnerBoundary.OrderBy(x => x.Yield).ElementAt(0);
            double annulusDensity = (double)data.anulares.ToList().Last().Densidad;

            return new CalculationElement
            {
                CollapsePressure = weakest.Colapso,
                BurstPressure = weakest.Yield,
                PressureGradient = (0.052 * annulusDensity),
                RuleCode = CalculationRulesCode.InnermostCasingOrTubing,
                RuleTitle = "Inner weakest element in Annulus",
                IsRelevant = true
            };
        }


        //Rule #2: Casing Analysis for each Annulus
        List<CalculationElement> GetExternalCasingAnalysis(WellPressureCalculationRequestDTO data)
        {
            List<Annulus> annulusWithContentsList = SchematicHelperFunctions.GetAnnulusContents(data.tubulares).ToList();
            List<CalculationElement> returnList = new List<CalculationElement>();

            int annulusIndex = 0;
            foreach (Annulus annulus in annulusWithContentsList)
            {
                Tubular weakestExternalElement = annulus.OuterBoundary.OrderBy(x => x.Yield).ElementAt(0);
                double annulusDensity = (double)data.anulares.ToList()[annulusIndex].Densidad;

                CalculationElement element = new CalculationElement
                {
                    CasingShoeTvd = weakestExternalElement.ProfundidadTVD,
                    CollapsePressure = weakestExternalElement.Colapso,
                    BurstPressure = weakestExternalElement.Yield,
                    BelowFormationFractureGradient = 0, //TODO: Get from exposed formations,
                    RuleTitle = "Tubing o Casing mas Externo del " + annulus.Anular,
                    RuleCode = CalculationRulesCode.MostExternalCasing,
                    PressureGradient = (0.052 * annulusDensity),
                    IsRelevant = true,
                };

                if (SchematicHelperFunctions.GetShallowestCementInAnnulus(annulus) != null)
                {
                    element.TopOfCementInAnular = (double)SchematicHelperFunctions.GetShallowestCementInAnnulus(annulus);
                }

                returnList.Add(element);
                annulusIndex++;

            }
            return returnList;
        }

        //Rule #3: Subsurface Safety Valve
        CalculationElement GetSubsurfaceSafetyValveAnalysis(WellPressureCalculationRequestDTO data)
        {
            List<Accessory> subSurfaceSafetyValves = data.accesorios.Where(x => x.Tipo == "SSSV").OrderBy(x => x.RatingDePresion).ToList();
            if (subSurfaceSafetyValves.Count == 0)
            {
                return new CalculationElement { IsRelevant = false, RuleCode = CalculationRulesCode.SubsurfaceSafetyValve, RuleTitle = "Subsurface Safety Valve" };
            }
            Accessory ssvData = subSurfaceSafetyValves[0];

            return new CalculationElement
            {
                RuleCode = CalculationRulesCode.SubsurfaceSafetyValve,
                RuleTitle = "Subsurface Safety Valve",
                MaxOperationRatingPressure = ssvData.RatingDePresion,
                ComponentTvd = ssvData.Profundidad,
                CollapsePressure = ssvData.RatingDePresion, //TODO:Es correcto ?
                IsRelevant = true
            };
        }

        //Rule #4: BHA Lowest Rating Accessory
        CalculationElement GetBhaLowestRatingAccessoryAnalysis(WellPressureCalculationRequestDTO data)
        {
            CalculationElement calculationElement = new CalculationElement
            {
                RuleCode = CalculationRulesCode.BhaLowestRatingAccessory,
                IsRelevant = false,
                RuleTitle = "Lowest Rating BHA Accessory"
            };

            //TODO: Implement

            return calculationElement;
        }

        //Rule #5: Prod/Iny Top Packer 
        CalculationElement GetTopPackerAnalysis(WellPressureCalculationRequestDTO data)
        {
            CalculationElement calculationElement = new CalculationElement
            {
                RuleCode = CalculationRulesCode.TopPackerAnalysis,
                IsRelevant = false,
                RuleTitle = "Top Packer"
            };

            //TODO: Implement

            return calculationElement;
        }

        //Rule #6: Top Liner Hanger Analysis
        CalculationElement GetTopLinerHangerAnalysis(WellPressureCalculationRequestDTO data)
        {
            CalculationElement calculationElement = new CalculationElement
            {
                RuleCode = CalculationRulesCode.TopLinerHangerAnalysis,
                IsRelevant = false,
                RuleTitle = "Liner Hanger"
            };

            //TODO: Implement

            return calculationElement;
        }

        //Rule #7: Wellhead Analysis
        List<CalculationElement> GetWellheadAnalysis(WellPressureCalculationRequestDTO data)
        {

            List<CalculationElement> returnList = new List<CalculationElement>();
            List<Annulus> annulusWithContentsList = SchematicHelperFunctions.GetAnnulusContents(data.tubulares).ToList();

            int annulusIndex = 0;
            foreach (Annulus annulus in annulusWithContentsList)
            {

                double annulusMaxOperPressure = data.cabezales.First(x => x.Anular == annulus.Anular).RatingDePresion;

                CalculationElement calculationElement = new CalculationElement
                {
                    RuleCode = CalculationRulesCode.WellheadAnalysis,
                    IsRelevant = true,
                    RuleTitle = "Wellhead analysis " + annulus.Anular,
                    MaxOperationRatingPressure = annulusMaxOperPressure,
                };

                returnList.Add(calculationElement);

                annulusIndex++;
            }
            return returnList;
        }
    }
}
