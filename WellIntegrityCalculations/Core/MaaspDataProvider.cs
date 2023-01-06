﻿using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Core
{
    public class MaaspDataProvider : IMaaspDataProvider
    {
        private ILogger<MaaspDataProvider> _logger;

        public MaaspDataProvider(ILogger<MaaspDataProvider> logger)
        {
            this._logger = logger;
        }

        public KeyValuePair<string, double> GetAnnulusA(List<CalculationElement> calculationRulesList, Dictionary<string, double> securityFactors)
        {
            _logger.LogInformation("Calculating MAASP for Annulus A");

            Dictionary<string, double> annulusAData = new Dictionary<string, double>();

            _logger.LogInformation("Annulus A - Point 1: SSSV Collapse");
            var point1 = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.SubsurfaceSafetyValve);

            if (point1.IsRelevant)
            {
                var mostInternalGradient = calculationRulesList.Find(x => x.RuleCode == CalculationRulesCode.InnermostCasingOrTubing).PressureGradient;
                var mostExternalGradient = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular A") > 0).PressureGradient;

                if (mostExternalGradient > mostInternalGradient)
                {
                    double ruleValue = point1.MaxOperationRatingPressure * securityFactors["Collapse"] - (point1.ComponentTvd * (mostExternalGradient - mostInternalGradient));
                    annulusAData.Add("1", ruleValue); //TODO: Get Collapse Pressure for SSSV
                }
                else
                {
                    annulusAData.Add("1", point1.MaxOperationRatingPressure * securityFactors["Collapse"]); //TODO: Get Collapse Pressure for SSSV
                }
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 1: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus A - Point 2: Collapse in Accessory");

            var point2 = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.BhaLowestRatingAccessory);

            if (point2.IsRelevant)
            {
                var mostInternalGradient = calculationRulesList.Find(x => x.RuleCode == CalculationRulesCode.InnermostCasingOrTubing).PressureGradient;
                var mostExternalGradient = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular A") > 0).PressureGradient;

                if (mostExternalGradient > mostInternalGradient)
                {
                    double ruleValue = point2.MaxOperationRatingPressure * securityFactors["Collapse"] - (point2.ComponentTvd * (mostExternalGradient - mostInternalGradient));
                    annulusAData.Add("2", ruleValue); //TODO: Get Collapse Pressure for Accessory
                }
                else
                {
                    annulusAData.Add("2", point2.MaxOperationRatingPressure * securityFactors["Collapse"]); //TODO: Get Collapse Pressure for Accessory
                }
            }
            else
            {
                _logger.LogInformation("Annulus A - Point B: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus A - Point 3: Collapse in Packer");
            var point3 = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.TopPackerAnalysis);

            if (point3.IsRelevant)
            {
                var mostInternalGradient = calculationRulesList.Find(x => x.RuleCode == CalculationRulesCode.InnermostCasingOrTubing).PressureGradient;
                var mostExternalGradient = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular A") > 0).PressureGradient;

                if (mostExternalGradient > mostInternalGradient)
                {
                    double ruleValue = point3.MaxOperationRatingPressure * securityFactors["Collapse"] - (point3.ComponentTvd * (mostExternalGradient - mostInternalGradient));
                    annulusAData.Add("3", ruleValue); //TODO: Get Collapse Pressure for Top Packer
                }
                else
                {
                    annulusAData.Add("3", point3.MaxOperationRatingPressure * securityFactors["Collapse"]); //TODO: Get Collapse Pressure for Top Packer
                }
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 3: Determined as Non-Relevant");
            }

            //Point 4

            _logger.LogInformation("Annulus A - Point 4A: Liner Hanger");
            var point4a = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.TopLinerHangerAnalysis);

            if (point4a.IsRelevant)
            {
                var mostExternalGradient = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular A") > 0).PressureGradient;

                double ruleValue = (point4a.MaxOperationRatingPressure + point4a.BelowFormationPressureBelow) -
                                   (point4a.ComponentTvd * mostExternalGradient); //TODO: To Be Defined
                annulusAData.Add("3", ruleValue); //TODO: Get Collapse Pressure for Top Packer
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 4A: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus A - Point 4B: Packer Burst");
            var point4b = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.TopPackerAnalysis);

            if (point4b.IsRelevant)
            {
                //TODO: Implement
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 4B: Determined as Non-Relevant");
            }

            //TODO: Implement 4C and 4D

            //Point 5

            _logger.LogInformation("Annulus A - Point 5: Collapse on Inner Elements");
            var point5 = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.InnermostCasingOrTubing);

            if (point5.IsRelevant)
            {
                var hasProdInyPacker = calculationRulesList.Find(x => x.RuleCode == CalculationRulesCode.TopPackerAnalysis);

                if (hasProdInyPacker != null)
                {
                    var mostInternalGradient = calculationRulesList.Find(x => x.RuleCode == CalculationRulesCode.InnermostCasingOrTubing).PressureGradient;
                    var mostExternalGradient = calculationRulesList.Find(
                        x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                             x.RuleTitle.IndexOf("Anular A") > 0).PressureGradient;

                    if (mostExternalGradient > mostInternalGradient)
                    {
                        double ruleValue = point5.CollapsePressure * securityFactors["Collapse"] - (hasProdInyPacker.ComponentTvd * (mostExternalGradient - mostInternalGradient));
                        annulusAData.Add("5", ruleValue);
                    }
                    else
                    {
                        annulusAData.Add("5", point5.CollapsePressure * securityFactors["Collapse"]);
                    }
                }
                else
                {
                    var ruleValue = securityFactors["Collapse"] * point5.CollapsePressure;
                    annulusAData.Add("5", ruleValue);
                }
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 5: Determined as Non-Relevant");
            }

            //Point 6

            _logger.LogInformation("Annulus A - Point 6: ");
            //TODO: TBD


            //Point 7A.1
            //TODO: TBD

            //Point 7A.2
            //TODO: TBD

            //Point 7B
            //TODO: TBD


            //Point 8
            _logger.LogInformation("Annulus A - Point 8: Wellhead");
            var point8 = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.WellheadAnalysis && x.RuleTitle.IndexOf("Anular A") > 0);

            if (point8.IsRelevant)
            {
                annulusAData.Add("8", point8.MaxOperationRatingPressure);
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 8: Determined as Non-Relevant");
            }


            return annulusAData.ToList().OrderBy(x => x.Value).ToList()[0];
        }

        public KeyValuePair<string, double> GetAnnulusB(List<CalculationElement> calculationRulesList, Dictionary<string, double> securityFactors)
        {
            Dictionary<string, double> annulusBData = new Dictionary<string, double>();
            var mostExternalCasing = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.MostExternalCasing && x.RuleTitle.IndexOf("Anular B") > 0);
            var mostExternalCasingAnnulusA = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.MostExternalCasing && x.RuleTitle.IndexOf("Anular A") > 0);
            var annulusCData = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.MostExternalCasing && x.RuleTitle.IndexOf("Anular C") > 0);


            _logger.LogInformation("Annulus B - Point 1: Fracture Gradient");
            //TODO:TBD


            //Point 2
            _logger.LogInformation("Annulus B - Point 2: Internal Casing Collapse");
            if (mostExternalCasingAnnulusA.TopOfCementInAnular - 32.5 > 0) //TODO: Reemplazar con Datum
            {
                var mostInternalGradient = mostExternalCasingAnnulusA.PressureGradient;
                var mostExternalGradient = mostExternalCasing.PressureGradient;

                if (mostExternalGradient > mostInternalGradient)
                {
                    if (mostExternalCasingAnnulusA.TopOfCementInAnular < mostExternalCasing.CasingShoeTvd)
                    {
                        var ruleValue = securityFactors["Collapse"] * mostExternalCasingAnnulusA.CollapsePressure - (mostExternalCasingAnnulusA.TopOfCementInAnular * (mostExternalGradient - mostInternalGradient));
                        annulusBData.Add("2", ruleValue);
                    }
                    else
                    {
                        var ruleValue = securityFactors["Collapse"] * mostExternalCasingAnnulusA.CollapsePressure - (mostExternalCasingAnnulusA.CasingShoeTvd * (mostExternalGradient - mostInternalGradient));
                        annulusBData.Add("2", ruleValue);
                    }
                }
                else
                {
                    annulusBData.Add("2", securityFactors["Collapse"] * mostExternalCasingAnnulusA.CollapsePressure);
                }
            }
            else
            {
                annulusBData.Add("2", securityFactors["Collapse"] * mostExternalCasingAnnulusA.CollapsePressure);
            }


            //Point 3
            _logger.LogInformation("Annulus B - Point 3: Casing Burst");
            if (annulusCData == null)
            {
                annulusBData.Add("3", securityFactors["Burst"] * mostExternalCasing.BurstPressure);
            }
            else
            {
                var annulusCPressureGradient = annulusCData.PressureGradient;
                if (mostExternalCasing.PressureGradient > annulusCPressureGradient)
                {
                    if (mostExternalCasing.TopOfCementInAnular < mostExternalCasing.CasingShoeTvd)
                    {
                        annulusBData.Add("3", securityFactors["Burst"] * mostExternalCasing.BurstPressure - (mostExternalCasing.TopOfCementInAnular * (mostExternalCasing.PressureGradient - annulusCPressureGradient)));
                    }
                    else
                    {
                        annulusBData.Add("3", securityFactors["Burst"] * mostExternalCasing.BurstPressure - (mostExternalCasing.CasingShoeTvd * (mostExternalCasing.PressureGradient - annulusCPressureGradient)));
                    }
                }
                else
                {
                    annulusBData.Add("3", securityFactors["Burst"] * mostExternalCasing.BurstPressure);
                }
            }

            //Point 4
            _logger.LogInformation("Annulus B - Point 4: Wellhead");
            var point4 = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.WellheadAnalysis && x.RuleTitle.IndexOf("Anular B") > 0);

            if (point4.IsRelevant)
            {
                annulusBData.Add("4", point4.MaxOperationRatingPressure);
            }
            else
            {
                _logger.LogInformation("Annulus B - Point 4: Determined as Non-Relevant");
            }



            return annulusBData.ToList().OrderBy(x => x.Value).ToList()[0];
        }

        public KeyValuePair<string, double> GetAnnulusC(List<CalculationElement> calculationRulesList, Dictionary<string, double> securityFactors)
        {

            Dictionary<string, double> annulusCData = new Dictionary<string, double>();
            var mostExternalCasing = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.MostExternalCasing && x.RuleTitle.IndexOf("Anular C") > 0);
            var mostExternalCasingAnnulusB = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.MostExternalCasing && x.RuleTitle.IndexOf("Anular B") > 0);
            var annulusDData = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.MostExternalCasing && x.RuleTitle.IndexOf("Anular D") > 0);

            _logger.LogInformation("Calculating MAASP for Annulus C");

            //Point 1
            _logger.LogInformation("Annulus C - Point 1: ");

            var belowFractureGradient = mostExternalCasing.BelowFormationFractureGradient;

            if (belowFractureGradient > mostExternalCasing.PressureGradient)
            {
                if (mostExternalCasingAnnulusB.TopOfCementInAnular < mostExternalCasing.CasingShoeTvd)
                {
                    annulusCData.Add("1", mostExternalCasingAnnulusB.TopOfCementInAnular * (belowFractureGradient - mostExternalCasing.PressureGradient));
                }
                else
                {
                    annulusCData.Add("1", mostExternalCasing.CasingShoeTvd * (belowFractureGradient - mostExternalCasing.PressureGradient));
                }
            }
            else
            {
                _logger.LogInformation("Annulus C - Point 1: Determined as Non-Relevant");
            }

            //Point 2
            _logger.LogInformation("Annulus C - Point 2: Inner Collapse");

            if (mostExternalCasing != null)
            {
                if (mostExternalCasing.PressureGradient > mostExternalCasingAnnulusB.PressureGradient)
                {
                    if (mostExternalCasingAnnulusB.TopOfCementInAnular < mostExternalCasing.CasingShoeTvd)
                    {
                        annulusCData.Add("2", (mostExternalCasingAnnulusB.CollapsePressure * securityFactors["Collapse"]) - (mostExternalCasing.CasingShoeTvd * (mostExternalCasing.PressureGradient - mostExternalCasingAnnulusB.PressureGradient)));
                    }
                    else
                    {
                        annulusCData.Add("2", (mostExternalCasingAnnulusB.CollapsePressure * securityFactors["Collapse"]) - (mostExternalCasingAnnulusB.TopOfCementInAnular * (mostExternalCasing.PressureGradient - mostExternalCasingAnnulusB.PressureGradient)));
                    }
                }
                else
                {
                    annulusCData.Add("2", mostExternalCasingAnnulusB.CollapsePressure * securityFactors["Collapse"]);
                }
            }
            else
            {
                _logger.LogInformation("Annulus C - Point 2: Determined as Non-Relevant");
            }

            //Point 3
            _logger.LogInformation("Annulus C - Point 3: Casing Burst");
            if (mostExternalCasing != null)
            {
                annulusCData.Add("3", mostExternalCasing.BurstPressure * securityFactors["Burst"]);
            }
            else
            {
                _logger.LogInformation("Annulus C - Point 3: Determined as Non-Relevant");
            }

            //Point 4
            _logger.LogInformation("Annulus C - Point 4: Maximum Working Pressure");
            if (mostExternalCasing != null)
            {
                var point4 = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.WellheadAnalysis && x.RuleTitle.IndexOf("Anular C") > 0);
                annulusCData.Add("4", point4.MaxOperationRatingPressure);
            }
            else
            {
                _logger.LogInformation("Annulus C - Point 4: Determined as Non-Relevant");
            }


            return annulusCData.ToList().OrderBy(x => x.Value).ToList()[0];
        }
    }
}