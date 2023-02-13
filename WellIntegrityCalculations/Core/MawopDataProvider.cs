using WellIntegrityCalculations.Models;
using WellIntegrityCalculations.Services;

namespace WellIntegrityCalculations.Core
{
    public class MawopDataProvider : IMawopDataProvider
    {
        private ILogger<IMawopDataProvider> _logger;

        public MawopDataProvider(ILogger<IMawopDataProvider> logger)
        {
            this._logger = logger;
        }
        public KeyValuePair<string, double> GetAnnulusA(List<CalculationElement> calculationRulesList)
        {
            _logger.LogInformation("Calculating MAWOP for Annulus A");
            Dictionary<string, double> annulusAData = new Dictionary<string, double>();

            _logger.LogInformation("Annulus A - Point 1A: Burst Pressure Most External Annulus A * 50%");
            CalculationElement point1ARule = calculationRulesList.FindAll(x => x.RuleCode == CalculationRulesCode.MostExternalCasing)[0];
            if (point1ARule.IsRelevant)
            {
                annulusAData.Add("1A", point1ARule.BurstPressure * 0.5);
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 1A: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus A - Point 1B: Burst Pressure Most External Annulus B * 80%");
            var point1BRule = calculationRulesList.FindAll(x => x.RuleCode == CalculationRulesCode.MostExternalCasing)[1];
            if (point1BRule.IsRelevant)
            {
                annulusAData.Add("1B", point1BRule.BurstPressure * 0.8);
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 1B: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus A - Point 2: Collapse Pressure Most Internal Annulus A * 75%");

            var point2Rule = calculationRulesList.Find(x => x.RuleCode == CalculationRulesCode.InnermostCasingOrTubing);

            if (point2Rule.IsRelevant && point2Rule.Diameter!= point1ARule.Diameter)
            {
                annulusAData.Add("2", point2Rule.CollapsePressure * 0.75);
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 2: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus A - Point 3: Max Oper Pressure on Wellhead * 80%");
            var point3Rule = calculationRulesList.FindAll(x => x.RuleCode == CalculationRulesCode.WellheadAnalysis &&
                                                                (x.RuleTitle.IndexOf("Anular A") > 0 || x.RuleTitle.IndexOf("Anular B")>0)
                                                          )
                                                 .OrderBy(x => x.MaxOperationRatingPressure)
                                                 .ToList()[0];

            if (point3Rule.IsRelevant)
            {
                annulusAData.Add("3", point3Rule.MaxOperationRatingPressure * 0.8);
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 3: Determined as Non-Relevant");
            }


            _logger.LogInformation("Annulus A - Point 4: TBD * 80%");
            var annulusAGradient = point1ARule.BelowFormationFractureGradient;
            var annulusBGradient = point1BRule.BelowFormationFractureGradient;




            _logger.LogInformation("Annulus A - Point 5A: Max Env Pressure in Inj/Prod packer * 80%");
            var point5aRule = calculationRulesList.Find(x => x.RuleCode == CalculationRulesCode.TopPackerAnalysis);
            if (point5aRule.IsRelevant)
            {
                annulusAData.Add("5A", point5aRule.MaxOperationRatingPressure * 0.8);
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 5A: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus A - Point 5B: Max SSSV * 80%");
            var point5bRule = calculationRulesList.Find(x => x.RuleCode == CalculationRulesCode.SubsurfaceSafetyValve);
            if (point5bRule.IsRelevant)
            {
                annulusAData.Add("5B", point5bRule.MaxOperationRatingPressure * 0.8);
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 5B: Determined as Non-Relevant");
            }


            _logger.LogInformation("Annulus A - Point 5C: Weakest Completion Component/Accesory * 80%");

            var mostExternalCasingAnnulusA = calculationRulesList.FindAll(x => x.RuleCode == CalculationRulesCode.MostExternalCasing)[0];
            var mostInternalComponentAnnulusA = calculationRulesList.Find(x => x.RuleCode == CalculationRulesCode.InnermostCasingOrTubing);
            var bhaRule = calculationRulesList.Find(x => x.RuleCode == CalculationRulesCode.BhaLowestRatingAccessory);

            if (bhaRule.IsRelevant)
            {
                if (mostExternalCasingAnnulusA.PressureGradient <= mostInternalComponentAnnulusA.PressureGradient)
                {
                    annulusAData.Add("5C", bhaRule.MaxOperationRatingPressure * 0.8);
                }
                else
                {
                    annulusAData.Add("5C", bhaRule.MaxOperationRatingPressure - (bhaRule.ComponentTvd * (mostExternalCasingAnnulusA.PressureGradient - mostInternalComponentAnnulusA.PressureGradient) * 0.8));
                }
            }
            else
            {
                _logger.LogInformation("Annulus A - Point 5C: Determined as Non-Relevant");
            }

            return annulusAData.ToList().OrderBy(x => x.Value).ToList()[0];
        }

        public KeyValuePair<string, double> GetAnnulusB(List<CalculationElement> calculationRulesList)
        {
            _logger.LogInformation("Calculating MAWOP for Annulus B");
            Dictionary<string, double> annulusBData = new Dictionary<string, double>();

            _logger.LogInformation("Annulus B - Point 1: Burst Pressure Most External Annulus B");
            var point1Rule = calculationRulesList.FindAll(x => x.RuleCode == CalculationRulesCode.MostExternalCasing)[1];
            bool isMostExternal = calculationRulesList.FindAll(x => x.RuleCode == CalculationRulesCode.MostExternalCasing && x.IsRelevant == true).Count() == 2;

            if (point1Rule.IsRelevant)
            {
                if (isMostExternal) annulusBData.Add("1", point1Rule.BurstPressure * 0.3);
                else annulusBData.Add("1", point1Rule.BurstPressure * 0.5);

            }
            else
            {
                _logger.LogInformation("Annulus B - Point 1: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus B - Point 2: Inner Element");
            var point2Rule = calculationRulesList.FindAll(x => x.RuleCode == CalculationRulesCode.MostExternalCasing)[0];

            if (point2Rule.IsRelevant)
            {
                annulusBData.Add("2", point2Rule.CollapsePressure * 0.75);

            }
            else
            {
                _logger.LogInformation("Annulus B - Point 2: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus B - Point 3: Wellhead");
            var point3Rule = calculationRulesList.FindAll(x =>
                            x.RuleCode == CalculationRulesCode.WellheadAnalysis
                            && (x.RuleTitle.IndexOf("Anular B") > 0 || x.RuleTitle.IndexOf("Anular C")>0)
                            ).OrderBy(x => x.MaxOperationRatingPressure).ToList()[0];

            if (point3Rule.IsRelevant)
            {
                annulusBData.Add("3", point3Rule.MaxOperationRatingPressure * 0.8);

            }
            else
            {
                _logger.LogInformation("Annulus B - Point 3: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus B - Point 4: ");
            //TODO: To Be Defined


            return annulusBData.ToList().OrderBy(x => x.Value).ToList()[0];
        }

        public KeyValuePair<string, double> GetAnnulusC(List<CalculationElement> calculationRulesList)
        {
            _logger.LogInformation("Calculating MAWOP for Annulus C");
            Dictionary<string, double> annulusCData = new Dictionary<string, double>();

            _logger.LogInformation("Annulus C - Point 1: Burst Pressure Most External Annulus C");
            var point1Rule = calculationRulesList.FindAll(x => x.RuleCode == CalculationRulesCode.MostExternalCasing)[2];

            bool isMostExternal = calculationRulesList.FindAll(x => x.RuleCode == CalculationRulesCode.MostExternalCasing).Count() == 3;

            if (point1Rule.IsRelevant)
            {
                if (isMostExternal) annulusCData.Add("1", point1Rule.BurstPressure * 0.3);
                else annulusCData.Add("1", point1Rule.BurstPressure * 0.5);
            }
            else
            {
                _logger.LogInformation("Annulus C - Point 1: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus C - Point 2: Collapse Pressure, Inner Casing");
            var point2Rule = calculationRulesList.FindAll(x => x.RuleCode == CalculationRulesCode.MostExternalCasing)[1];
            if (point2Rule.IsRelevant)
            {
                annulusCData.Add("2", point2Rule.CollapsePressure * 0.75);
            }
            else
            {
                _logger.LogInformation("Annulus C - Point 2: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus C - Point 3: Max Operating Rating Pressure Wellhead");
            var point3Rule = calculationRulesList.FindAll(x =>
                            x.RuleCode == CalculationRulesCode.WellheadAnalysis
                            && (x.RuleTitle.IndexOf("Anular C") > 0)).OrderBy(x => x.MaxOperationRatingPressure).ToList()[0];

            if (point3Rule.IsRelevant)
            {
                annulusCData.Add("3", point3Rule.MaxOperationRatingPressure * 0.8);
            }
            else
            {
                _logger.LogInformation("Annulus B - Point 3: Determined as Non-Relevant");
            }

            _logger.LogInformation("Annulus B - Point 4: ");
            //TODO: To Be Defined



            return annulusCData.ToList().OrderBy(x => x.Value).ToList()[0];
        }

        public KeyValuePair<string, double> GetAnnulusD(List<CalculationElement> calculationRulesList)
        {
            throw new NotImplementedException();
        }
    }
}
