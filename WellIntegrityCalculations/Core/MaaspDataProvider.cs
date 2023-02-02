using Microsoft.AspNetCore.DataProtection.XmlEncryption;
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

        public KeyValuePair<string, double> GetAnnulusA(List<CalculationElement> calculationRulesList, Dictionary<string, double> securityFactors, DatumData datumData)
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
                    double ruleValue = point1.CollapsePressure* securityFactors["Collapse"] - (point1.ComponentTvd * (mostExternalGradient - mostInternalGradient));
                    annulusAData.Add("1", ruleValue);
                }
                else
                {
                    annulusAData.Add("1", point1.CollapsePressure * securityFactors["Collapse"]);
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
                    double ruleValue = point2.CollapsePressure * securityFactors["Collapse"] - (point2.ComponentTvd * (mostExternalGradient - mostInternalGradient));
                    annulusAData.Add("2", ruleValue);
                }
                else
                {
                    annulusAData.Add("2", point2.CollapsePressure * securityFactors["Collapse"]);
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
                                   (point4a.ComponentTvd * mostExternalGradient) - (point4a.PressureGradient * (point4a.BelowFormationDepth - point4a.ComponentTvd));

                annulusAData.Add("4A", ruleValue); 
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 4A: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus A - Point 4B: Packer Burst");
            var point4b = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.TopLinerHangerAnalysis);

            if (point4b.IsRelevant)
            {
                var mostExternalGradientA = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular A") > 0).PressureGradient;

                var mostExternalGradientB = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular B") > 0).PressureGradient;

                if (mostExternalGradientA > mostExternalGradientB)
                {
                    annulusAData.Add("4B", securityFactors["Burst"] * point4b.BurstPressure - (point4b.ComponentTvd*(mostExternalGradientA-mostExternalGradientB)));
                }
                else
                {
                    annulusAData.Add("4B", securityFactors["Burst"]*point4b.BurstPressure);
                }
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 4B: Determined as Non-Relevant");
            }


            _logger.LogInformation("Annulus A - Point 4C: Bottom Liner Hanger");
            var point4c = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.BottomLinerHangerAnalysis);

            if (point4c.IsRelevant)
            {
                var mostExternalAnnulusAGradient = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular A") > 0).PressureGradient;

                annulusAData.Add("4C", point4c.MaxOperationRatingPressure+point4c.BelowFormationPressureBelow - (point4c.ComponentTvd * mostExternalAnnulusAGradient) - (point4c.BelowFormationFractureGradient*(point4c.BelowFormationDepth - point4c.ComponentTvd)));
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 4C: Determined as Non-Relevant");
            }


            _logger.LogInformation("Annulus A - Point 4D: Bottom Liner Hanger");
            var point4d = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.BottomLinerHangerAnalysis);

            if (point4d.IsRelevant)
            {
                var mostExternalAnnulusAGradient = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular A") > 0).PressureGradient;

                var mostExternalAnnulusBGradient = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular B") > 0).PressureGradient;


                if (mostExternalAnnulusAGradient > mostExternalAnnulusBGradient)
                {
                    annulusAData.Add("4D", securityFactors["Burst"] * point4d.BurstPressure - (point4d.ComponentTvd * (mostExternalAnnulusAGradient-mostExternalAnnulusBGradient)));
                }
                else
                {
                    annulusAData.Add("4D", securityFactors["Burst"] * point4d.BurstPressure);
                }
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 4d: Determined as Non-Relevant");
            }



            //Point 5

            _logger.LogInformation("Annulus A - Point 5: Collapse on Inner Elements");
            var point5 = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.InnermostCasingOrTubing);

            if (point5.IsRelevant && point5.Diameter != calculationRulesList.FindAll(x => x.RuleCode == CalculationRulesCode.MostExternalCasing)[0].Diameter)
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
            _logger.LogInformation("Annulus A - Point 6: Fracture");
            var point6 = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.TopLinerHangerAnalysis);

            if (point6.IsRelevant)
            {
                var mostExternalA = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular A") > 0);

                if (point6.BelowFormationFractureGradient > mostExternalA.PressureGradient)
                {
                    annulusAData.Add("6", mostExternalA.CasingShoeTvd*(point6.BelowFormationFractureGradient - mostExternalA.PressureGradient));
                }
                else
                {
                    _logger.LogInformation("Annulus A - Point 6: Determined as Non-Relevant");
                }
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 6: Determined as Non-Relevant");
            }

            //Point 7A.1
            //TODO: TBD

            //Point 7A.2
            _logger.LogInformation("Annulus A - Point 7A.2: External Casing Bust @ Liner Hanger Depth");
            var point7a2 = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.TopLinerHangerAnalysis);

            if (point7a2.IsRelevant)
            {
                var mostExternalA = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular A") > 0);

                var mostExternalB = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular B") > 0);

                if (mostExternalA.PressureGradient > mostExternalB.PressureGradient)
                {
                    annulusAData.Add("7A.2", securityFactors["Burst"] * mostExternalA.BurstPressure - (point4b.ComponentTvd * (mostExternalA.PressureGradient - mostExternalB.PressureGradient)));
                }
                else
                {
                    annulusAData.Add("7A.2", securityFactors["Burst"] * mostExternalA.BurstPressure);
                }
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 7A.2: Determined as Non-Relevant");
            }

            //Point 7B
            _logger.LogInformation("Annulus A - Point 7B: Liner Overlap Burst");
            var point7b = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.TopLinerHangerAnalysis);
            var packer = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.TopPackerAnalysis);

            if (point7b.IsRelevant)
            {
                var mostExternalA = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular A") > 0);

                var mostExternalB = calculationRulesList.Find(
                    x => x.RuleCode == CalculationRulesCode.MostExternalCasing &&
                         x.RuleTitle.IndexOf("Anular B") > 0);

                if (point7b.PressureGradient > mostExternalA.PressureGradient)
                {
                    annulusAData.Add("7B", mostExternalA.BurstPressure + 
                                           point7b.BelowFormationPressureBelow - 
                                           (point7b.ComponentTvd * mostExternalA.PressureGradient) - 
                                           point7b.PressureGradient * (point7b.BelowFormationDepth - point7b.ComponentTvd));
                }
                else
                {
                    annulusAData.Add("7B", mostExternalA.BurstPressure + point7b.BelowFormationPressureBelow - (packer.ComponentTvd*mostExternalA.PressureGradient)- point7b.PressureGradient * (point7b.BelowFormationDepth - packer.ComponentTvd));
                }
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 7B: Determined as Non-Relevant");
            }



            //Point 8
            _logger.LogInformation("Annulus A - Point 8: Wellhead");
            var point8 = calculationRulesList.FindAll(x => x.RuleCode == CalculationRulesCode.WellheadAnalysis &&
                                                                (x.RuleTitle.IndexOf("Anular A") > 0 || x.RuleTitle.IndexOf("Anular B") > 0) &&
                                                                x.IsRelevant == true
                                                          )
                                                 .OrderBy(x => x.MaxOperationRatingPressure)
                                                 .ToList()[0];

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

        public KeyValuePair<string, double> GetAnnulusB(List<CalculationElement> calculationRulesList, Dictionary<string, double> securityFactors, DatumData datumData)
        {
            Dictionary<string, double> annulusBData = new Dictionary<string, double>();
            var mostExternalCasing = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.MostExternalCasing && x.RuleTitle.IndexOf("Anular B") > 0);
            var mostExternalCasingAnnulusA = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.MostExternalCasing && x.RuleTitle.IndexOf("Anular A") > 0);
            var annulusCData = calculationRulesList.Find((x) => x.RuleCode == CalculationRulesCode.MostExternalCasing && x.RuleTitle.IndexOf("Anular C") > 0);


            _logger.LogInformation("Annulus B - Point 1: Fracture Gradient");
            if(mostExternalCasing.BelowFormationFractureGradient > mostExternalCasing.PressureGradient)
            {
                if(mostExternalCasing.TopOfCementInAnular < mostExternalCasing.CasingShoeTvd)
                {
                    annulusBData.Add("1", mostExternalCasing.TopOfCementInAnular * (mostExternalCasing.BelowFormationFractureGradient - mostExternalCasing.PressureGradient));
                }
                else
                {
                    annulusBData.Add("1", mostExternalCasing.CasingShoeTvd * (mostExternalCasing.BelowFormationFractureGradient - mostExternalCasing.PressureGradient));
                }
            }
            else
            {
                _logger.LogInformation("Annulus B - Point 1: Determined as Non-Relevant");
            }


            //Point 2
            _logger.LogInformation("Annulus B - Point 2: Internal Casing Collapse");
            if (mostExternalCasing.TopOfCementInAnular - datumData.AirGap> 0)
            {
                var mostInternalGradient = mostExternalCasingAnnulusA.PressureGradient;
                var mostExternalGradient = mostExternalCasing.PressureGradient;

                if (mostExternalGradient > mostInternalGradient)
                {
                    if (mostExternalCasingAnnulusA.TopOfCementInAnular < mostExternalCasing.CasingShoeTvd)
                    {
                        var ruleValue = securityFactors["Collapse"] * mostExternalCasingAnnulusA.CollapsePressure - (mostExternalCasing.TopOfCementInAnular * (mostExternalGradient - mostInternalGradient));
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
                    if (annulusCData.TopOfCementInAnular < mostExternalCasing.CasingShoeTvd)
                    {
                        annulusBData.Add("3", securityFactors["Burst"] * mostExternalCasing.BurstPressure - (annulusCData.TopOfCementInAnular * (mostExternalCasing.PressureGradient - annulusCPressureGradient)));
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
            var point4 = calculationRulesList.FindAll(x =>
                            x.RuleCode == CalculationRulesCode.WellheadAnalysis
                            && (x.RuleTitle.IndexOf("Anular B") > 0 || x.RuleTitle.IndexOf("Anular C") > 0)
                            ).OrderBy(x => x.MaxOperationRatingPressure).ToList()[0];

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

        public KeyValuePair<string, double> GetAnnulusC(List<CalculationElement> calculationRulesList, Dictionary<string, double> securityFactors, DatumData datumData)
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
