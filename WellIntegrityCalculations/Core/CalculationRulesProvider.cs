using System.Data;
using System.Xml.Linq;
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
            calculationElements.Add(GetBottomLinerHangerAnalysis(data));
            calculationElements.AddRange(GetWellheadAnalysis(data));

            return calculationElements;
        }

        //Rule #1: Inner Weakest Element in Annulus
        CalculationElement GetInnerWeakestElementInAnnulus(WellPressureCalculationRequestDTO data)
        {
            List<Annulus> annulusWithContentsList = SchematicHelperFunctions.GetAnnulusContents(data.tubulares, data.ReferenceDepths).ToList();

            CalculationElement calcElem = new CalculationElement() { IsRelevant = true };

            Tubular weakest;
            if (annulusWithContentsList[0].InnerBoundary.Count == 0)
            {
                weakest = annulusWithContentsList[0].OuterBoundary.OrderBy(x => x.Diameter).First();
            }
            else
            {
                weakest = annulusWithContentsList[0].InnerBoundary.OrderBy(x => x.Yield).ElementAt(0);
            }
            double annulusDensity = (double)data.anulares.ToList().Last().Densidad;

            calcElem.CollapsePressure = weakest.Colapso;
            calcElem.BurstPressure = weakest.Yield;
            calcElem.PressureGradient = (0.052 * annulusDensity);
            calcElem.Diameter = weakest.Diameter;
            calcElem.RuleCode = CalculationRulesCode.InnermostCasingOrTubing;
            calcElem.RuleTitle = "Inner weakest element in Annulus";

            return calcElem;
        }


        //Rule #2: Casing Analysis for each Annulus
        List<CalculationElement> GetExternalCasingAnalysis(WellPressureCalculationRequestDTO data)
        {
            List<Annulus> annulusWithContentsList = SchematicHelperFunctions.GetAnnulusContents(data.tubulares, data.ReferenceDepths).ToList();
            List<CalculationElement> returnList = new List<CalculationElement>();

            int annulusIndex = 0;
            foreach (Annulus annulus in annulusWithContentsList)
            {
                Tubular weakestExternalElement = annulus.OuterBoundary.ToList().FindAll(x => x.Liner == "NO").OrderBy(x => x.Colapso).ElementAt(0);
                double annulusDensity = (double)data.anulares.ToList()[annulusIndex].Densidad;

                CalculationElement element = new CalculationElement
                {
                    CasingShoeTvd = weakestExternalElement.ProfundidadTVD,
                    CollapsePressure = weakestExternalElement.Colapso,
                    BurstPressure = weakestExternalElement.Yield,
                    RuleTitle = "Tubing o Casing mas Externo del " + annulus.Anular,
                    RuleCode = CalculationRulesCode.MostExternalCasing,
                    PressureGradient = (0.052 * annulusDensity),
                    Diameter = (double)weakestExternalElement.Diameter,
                    IsRelevant = weakestExternalElement.AssemblyName != "CONDUCTOR" && annulusIndex < data.anulares.ToList().FindAll(x => x.Anular.Contains("Anular")).Count,
                };

                if(annulus.Anular =="Anular A")
                {
                    List<Accessory> relevantPackers = data.accesorios.Where(x => x.Tipo == "PKR" && x.RatingDePresion > 0).OrderBy(x => x.RatingDePresion).ToList();
                    List<Perforation> openPerforations = data.Perforations.ToList().FindAll(x => x.Status == "OPEN");
                    if (relevantPackers.Count == 0 && openPerforations.Count() > 0)
                    {
                        double minFractureGradient = double.MaxValue;

                        foreach (var perforation in openPerforations)
                        {
                            minFractureGradient = Math.Min(minFractureGradient, SchematicHelperFunctions.GetMinimumFractureGradientInPerforations(data.Survey,
                                data.ReferenceDepths,
                                perforation,
                                data.FracturePressureGradient,
                                data.formaciones));
                        }

                        element.BelowFormationFractureGradient = minFractureGradient;
                    }
                    else if (relevantPackers.Count > 0 && openPerforations.Count>0)
                    {
                        List<Perforation> perforationsAbovePacker = data.Perforations.ToList().FindAll(x => x.Status == "OPEN" && x.EndMD < relevantPackers.OrderBy(x=>x.Profundidad).First().Profundidad);
                        double minFractureGradient = double.MaxValue;

                        if(perforationsAbovePacker.Count > 0)
                        {
                            foreach (var perforation in perforationsAbovePacker)
                            {
                                minFractureGradient = Math.Min(minFractureGradient, SchematicHelperFunctions.GetMinimumFractureGradientInPerforations(data.Survey,
                                    data.ReferenceDepths,
                                    perforation,
                                    data.FracturePressureGradient,
                                    data.formaciones));
                            }

                            element.BelowFormationFractureGradient = minFractureGradient;
                        }
                    }
                }
                else if (SchematicHelperFunctions.GetShallowestCementInAnnulus(annulus) != null)
                {
                    element.TopOfCementInAnular = SchematicHelperFunctions.GetInterpolatedTvd(data.Survey, data.ReferenceDepths, (double)SchematicHelperFunctions.GetShallowestCementInAnnulus(annulus));
                    if (element.TopOfCementInAnular != null) element.TopOfCementInAnular = (double)(((double)element.TopOfCementInAnular) < data.ReferenceDepths.AirGap ? 0 : ((double)element.TopOfCementInAnular - data.ReferenceDepths.AirGap));

                    element.BelowFormationFractureGradient = SchematicHelperFunctions.GetFractureGradientInAnnulus(element.CasingShoeTvd, element.TopOfCementInAnular, data.FracturePressureGradient, data.formaciones);
                }

                returnList.Add(element);
                annulusIndex++;

            }
            return returnList;
        }

        //Rule #3: Subsurface Safety Valve
        CalculationElement GetSubsurfaceSafetyValveAnalysis(WellPressureCalculationRequestDTO data)
        {
            List<Accessory> subSurfaceSafetyValves = data.accesorios.Where(x => x.Tipo == "SSSV" 
            && (x.Profundidad +  data.ReferenceDepths.DatumElevation - data.ReferenceDepths.AirGap) <=500)
                .OrderBy(x => x.RatingDePresion).ToList();
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
                ComponentTvd = SchematicHelperFunctions.GetInterpolatedTvd(data.Survey, data.ReferenceDepths, ssvData.Profundidad + (double)data.ReferenceDepths.DatumElevation),
                CollapsePressure = ssvData.CollapsePressure, //TODO:Es correcto ?
                IsRelevant = true
            };
        }

        //Rule #4: BHA Lowest Rating Accessory
        CalculationElement GetBhaLowestRatingAccessoryAnalysis(WellPressureCalculationRequestDTO data)
        {
            List<Accessory> relevantBhaAccessories = data.accesorios.Where(x =>
            (!(x.Tipo == "TBG" && x.CompType == "TBG") ||
            !(x.Tipo == "TBG" && x.CompType == "LH"))

            && x.RatingDePresion > 0 && x.Tipo != "SSSV" && x.Tipo != "PKR").OrderBy(x => x.RatingDePresion).ToList();
            if (relevantBhaAccessories.Count == 0)
            {
                return new CalculationElement()
                {
                    RuleCode = CalculationRulesCode.BhaLowestRatingAccessory,
                    IsRelevant = false,
                    RuleTitle = "Lowest Rating BHA Accessory"
                };
            }
            Accessory weakestBhaElem = relevantBhaAccessories[0];

            CalculationElement calculationElement = new CalculationElement
            {
                RuleCode = CalculationRulesCode.BhaLowestRatingAccessory,
                IsRelevant = relevantBhaAccessories.Count > 0,
                RuleTitle = "Lowest Rating BHA Accessory",
                ComponentTvd = SchematicHelperFunctions.GetInterpolatedTvd(data.Survey, data.ReferenceDepths, weakestBhaElem.Profundidad + (double)data.ReferenceDepths.DatumElevation - (double)data.ReferenceDepths.AirGap),
                MaxOperationRatingPressure = weakestBhaElem.RatingDePresion,
                CollapsePressure = weakestBhaElem.CollapsePressure,
            };

            return calculationElement;
        }

        //Rule #5: Prod/Iny Top Packer 
        CalculationElement GetTopPackerAnalysis(WellPressureCalculationRequestDTO data)
        {
            List<Accessory> relevantPackers = data.accesorios.Where(x => x.Tipo == "PKR" && x.RatingDePresion > 0).OrderBy(x => x.Profundidad).ToList();
            if (relevantPackers.Count == 0)
            {
                return new CalculationElement()
                {
                    RuleCode = CalculationRulesCode.TopPackerAnalysis,
                    IsRelevant = false,
                    RuleTitle = "Top Packer"
                };
            }

            Accessory topPacker = relevantPackers[0];

            CalculationElement calculationElement = new CalculationElement
            {
                RuleCode = CalculationRulesCode.TopPackerAnalysis,
                IsRelevant = true,
                RuleTitle = "Top Packer",
                ComponentTvd = SchematicHelperFunctions.GetInterpolatedTvd(data.Survey, data.ReferenceDepths, topPacker.Profundidad + (double)data.ReferenceDepths.DatumElevation - (double)data.ReferenceDepths.AirGap),
                MaxOperationRatingPressure = topPacker.RatingDePresion,
                CollapsePressure = topPacker.CollapsePressure,
            };

            List<Perforation> openPerforations = data.Perforations.ToList().FindAll(x => x.Status == "OPEN");
            List<Perforation> perforationsBelowPacker = data.Perforations.ToList().FindAll(x => x.Status == "OPEN" && x.StartMD > topPacker.Profundidad);

            if (perforationsBelowPacker.Count > 0)
            {
                double minFormationPressure = double.MaxValue;
                double minPorePressureGradient = 0.433;
                double belowFormationDepth = 0;

                foreach (var perforation in perforationsBelowPacker)
                {
                    double minimumPressGradInPerforation = SchematicHelperFunctions.GetMinimumPressureGradientInPerforations(data.Survey,
                        data.ReferenceDepths,
                        perforation,
                        data.PorePressureGradient,
                        data.formaciones);

                    if (minimumPressGradInPerforation < minFormationPressure)
                    {
                        minFormationPressure = minimumPressGradInPerforation;
                        minPorePressureGradient = minimumPressGradInPerforation / SchematicHelperFunctions.GetInterpolatedTvd(data.Survey, data.ReferenceDepths, perforation.StartMD);
                        belowFormationDepth = SchematicHelperFunctions.GetInterpolatedTvd(data.Survey, data.ReferenceDepths, perforation.StartMD+data.ReferenceDepths.DatumElevation);
                    }
                }

                calculationElement.BelowFormationPressureBelow = minFormationPressure;
                calculationElement.PressureGradient = minPorePressureGradient;
                calculationElement.BelowFormationDepth = belowFormationDepth;
            }

            return calculationElement;
        }

        //Rule #6.1: Top Liner Hanger Analysis
        CalculationElement GetTopLinerHangerAnalysis(WellPressureCalculationRequestDTO data)
        {
            var linerAsmCount = data.Liner_Hanger.DistinctBy(x => x.AssemblyAlQuePertenece).Count();

            if (linerAsmCount == 0)
            {
                return new CalculationElement()
                {
                    RuleCode = CalculationRulesCode.TopLinerHangerAnalysis,
                    IsRelevant = data.Liner_Hanger.Count() != 0,
                    RuleTitle = "Top - Liner Hanger"
                };
            }

            LinerHanger upperLinerHanger = data.Liner_Hanger.ToList().OrderBy(x => x.ProfundidadMd).ToList().First();
            Tubular linerData = data.tubulares.First(x => x.AssemblyName == upperLinerHanger.AssemblyAlQuePertenece);
            double linerTop = SchematicHelperFunctions.GetInterpolatedTvd(data.Survey, data.ReferenceDepths, (double)upperLinerHanger.ProfundidadMd);
            double? actualToc = linerData.TocTVD == null ? null : SchematicHelperFunctions.GetInterpolatedTvd(data.Survey, data.ReferenceDepths, (double)linerData.TopeDeCemento);

            Annulus annulusA = SchematicHelperFunctions.GetAnnulusContents(data.tubulares, data.ReferenceDepths).ToList()[0];
            Tubular nextTubular = annulusA.OuterBoundary[annulusA.OuterBoundary.ToList().FindLastIndex(x => x.AssemblyName == linerData.AssemblyName) + 1];

            var openFormations = data.formaciones.ToList().FindAll(x => x.TvdTope < (actualToc ?? Double.MaxValue) && x.TvdBase > nextTubular.ProfundidadTVD && actualToc > nextTubular.ProfundidadTVD);

            CalculationElement calculationElement = new CalculationElement
            {
                RuleCode = CalculationRulesCode.TopLinerHangerAnalysis,
                IsRelevant = data.Liner_Hanger.Count() != 0,
                ComponentTvd = linerTop,
                MaxOperationRatingPressure = (double)upperLinerHanger.RatingDePresion,
                BurstPressure = (double)upperLinerHanger.BurstPressure,
                BelowFormationPressureBelow = 0,
                PressureGradient = 0,
                BelowFormationDepth = 0, 
                BelowFormationFractureGradient = SchematicHelperFunctions.GetFractureGradientInAnnulus(nextTubular.ProfundidadTVD, (actualToc ?? Double.MaxValue), data.FracturePressureGradient, data.formaciones),
                RuleTitle = "Liner Hanger"
            };

            if (openFormations.Count > 0)
            {
                double highestPP = 0;
                Formation criticalFormation = null;

                openFormations.ForEach(x =>
                {
                    var gradPoint = data.PorePressureGradient.ToList().Find(grad => x.Formacion == grad.formationname);
                    double gradPointPP = 0.433;
                    if (gradPoint != null) gradPointPP = (double)(gradPoint.value / (gradPoint.depth_tvd + data.ReferenceDepths.DatumElevation));

                    if (gradPointPP > highestPP)
                    {
                        highestPP = gradPointPP;
                        criticalFormation = x;
                    }
                });

                double openFormationDepth;
                if (criticalFormation.TvdTope < nextTubular.ProfundidadTVD) openFormationDepth = nextTubular.ProfundidadTVD;
                else openFormationDepth = (double)criticalFormation.TvdTope;

                calculationElement.BelowFormationPressureBelow = openFormationDepth * highestPP;
                calculationElement.PressureGradient = highestPP;
                calculationElement.BelowFormationDepth = openFormationDepth;
            }

            return calculationElement;
        }

        //Rule #6.2: Bottom Liner Hanger Analysis
        CalculationElement GetBottomLinerHangerAnalysis(WellPressureCalculationRequestDTO data)
        {
            var linerAsmCount = data.Liner_Hanger.DistinctBy(x => x.AssemblyAlQuePertenece).Count();

            if (linerAsmCount <= 1)
            {
                return new CalculationElement()
                {
                    RuleCode = CalculationRulesCode.BottomLinerHangerAnalysis,
                    IsRelevant = false,
                    RuleTitle = "Bottom - Liner Hanger"
                };
            }

            LinerHanger bottomLinerHanger = data.Liner_Hanger.MinBy((lh) =>
            {
                Tubular linerData = data.tubulares.First(x => x.AssemblyName == lh.AssemblyAlQuePertenece);
                return linerData.Diameter;
            });

            Tubular linerData = data.tubulares.First(x => x.AssemblyName == bottomLinerHanger.AssemblyAlQuePertenece);
            double linerTop = SchematicHelperFunctions.GetInterpolatedTvd(data.Survey, data.ReferenceDepths, (double)bottomLinerHanger.ProfundidadMd);
            double? actualToc = linerData.TocTVD == null ? null : SchematicHelperFunctions.GetInterpolatedTvd(data.Survey, data.ReferenceDepths, (double)linerData.TopeDeCemento);

            Annulus annulusA = SchematicHelperFunctions.GetAnnulusContents(data.tubulares, data.ReferenceDepths).ToList()[0];
            Tubular nextTubular = annulusA.OuterBoundary[annulusA.OuterBoundary.ToList().FindLastIndex(x => x.AssemblyName == linerData.AssemblyName) + 1];

            var openFormations = data.formaciones.ToList().FindAll(x => x.TvdTope < (actualToc ?? Double.MaxValue) && x.TvdBase > nextTubular.ProfundidadTVD && actualToc > nextTubular.ProfundidadTVD);

            CalculationElement calculationElement = new CalculationElement
            {
                RuleCode = CalculationRulesCode.BottomLinerHangerAnalysis,
                IsRelevant = data.Liner_Hanger.Count() >= 2,
                ComponentTvd = linerTop,
                MaxOperationRatingPressure = (double)bottomLinerHanger.RatingDePresion,
                BurstPressure = (double)bottomLinerHanger.BurstPressure,
                BelowFormationPressureBelow = 0,
                PressureGradient = 0,
                BelowFormationDepth = 0,
                BelowFormationFractureGradient = SchematicHelperFunctions.GetFractureGradientInAnnulus(nextTubular.ProfundidadTVD, (actualToc ?? Double.MaxValue), data.FracturePressureGradient, data.formaciones),
                RuleTitle = "Liner Hanger"
            };

            if (openFormations.Count > 0)
            {
                double highestPP = 0;
                Formation criticalFormation = null;

                openFormations.ForEach(x =>
                {
                    var gradPoint = data.PorePressureGradient.ToList().Find(grad => x.Formacion == grad.formationname);
                    double gradPointPP = 0.433;
                    if (gradPoint != null) gradPointPP = (double)(gradPoint.value / (gradPoint.depth_tvd + data.ReferenceDepths.DatumElevation));

                    if (gradPointPP > highestPP)
                    {
                        highestPP = gradPointPP;
                        criticalFormation = x;
                    }
                });

                double openFormationDepth;
                if (criticalFormation.TvdTope < nextTubular.ProfundidadTVD) openFormationDepth = nextTubular.ProfundidadTVD;
                else openFormationDepth = (double)criticalFormation.TvdTope;

                calculationElement.BelowFormationPressureBelow = openFormationDepth * highestPP;
                calculationElement.PressureGradient = highestPP;
                calculationElement.BelowFormationDepth = openFormationDepth;
            }

            return calculationElement;
        }

        //Rule #7: Wellhead Analysis
        List<CalculationElement> GetWellheadAnalysis(WellPressureCalculationRequestDTO data)
        {

            List<CalculationElement> returnList = new List<CalculationElement>();
            List<Annulus> annulusWithContentsList = SchematicHelperFunctions.GetAnnulusContents(data.tubulares, data.ReferenceDepths).ToList();

            int annulusIndex = 0;
            foreach (Annulus annulus in annulusWithContentsList)
            {

                double? annulusMaxOperPressure = data.cabezales.First(x => x.Anular == annulus.Anular).RatingDePresion;

                CalculationElement calculationElement = new CalculationElement
                {
                    RuleCode = CalculationRulesCode.WellheadAnalysis,
                    IsRelevant = annulusMaxOperPressure != null,
                    RuleTitle = "Wellhead analysis " + annulus.Anular,
                    MaxOperationRatingPressure = annulusMaxOperPressure ?? 0,
                };

                returnList.Add(calculationElement);

                annulusIndex++;
            }
            return returnList;
        }
    }
}
