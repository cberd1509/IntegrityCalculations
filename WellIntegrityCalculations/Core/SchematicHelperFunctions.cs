using System.Text.Json;
using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Core
{
    public static class SchematicHelperFunctions
    {
        /// <summary>
        /// Helper Constant with the Alphabet in Uppercase
        /// </summary>
        public const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Returns the list of Annulus and it's contents on each boundary
        /// </summary>
        /// <param name="casingData">Casing and tubular information of the Well Schematic</param>
        /// <returns>Annulus contents</returns>
        public static IEnumerable<Annulus> GetAnnulusContents(IEnumerable<Tubular> casingData, DatumData datumData)
        {
            List<Annulus> res = new List<Annulus>();
            bool syntethicTubingAdded = false;

            //It there's no Tubing, we should add a temporary 
            if (casingData.FirstOrDefault(x => x.SectType == "TBG") == null)
            {
                casingData = casingData.Prepend(new Tubular { AssemblyName = "Synthetic Tubing", SectType = "TBG", Profundidad = -1 }).ToList();
                syntethicTubingAdded = true;
            }

            Queue<Tubular> casingDataQueue = new Queue<Tubular>(casingData);

            int annulusIndex = 1;
            Annulus tempAnnulus = new Annulus { index = annulusIndex, InnerBoundary = new List<Tubular>(), OuterBoundary = new List<Tubular>(), Anular = "Anular " + ALPHABET[annulusIndex - 1] };
            Tubular? tempElem = null;

            while (casingDataQueue.Count > 0)
            {
                if (tempAnnulus.InnerBoundary.Count == 0)
                {
                    //Find Inner Boundary
                    do
                    {
                        tempElem = casingDataQueue.Dequeue();
                        tempAnnulus.InnerBoundary.Add(tempElem);

                        if (tempAnnulus.Anular == "Anular A" && casingDataQueue.ToList().FindAll(x => x.SectType == "TBG").Count == 0)
                        {
                            break;
                        }

                    } while ( Math.Round(Convert.ToDecimal(tempElem.TopeDeCasing),1) - Convert.ToDecimal(datumData.AirGap) > 0);
                }

                //Find Outer Boundary
                do
                {
                    tempElem = casingDataQueue.Dequeue();
                    tempAnnulus.OuterBoundary.Add(tempElem);
                } while (Math.Round(Convert.ToDecimal(tempElem.TopeDeCasing),1) - Convert.ToDecimal(datumData.AirGap) > 0);

                res.Add(tempAnnulus);
                annulusIndex++;
                tempAnnulus = new Annulus { index = annulusIndex, InnerBoundary = tempAnnulus.OuterBoundary, OuterBoundary = new List<Tubular>(), Anular = "Anular " + ALPHABET[annulusIndex - 1] };

            }
            if (syntethicTubingAdded)
            {
                res[0].InnerBoundary = new List<Tubular>();
            }

            res = res.FindAll(x => x.OuterBoundary.ToList().Find(x => x.AssemblyName.ToUpper().Contains("CONDUCTOR")) == null);
            res = res.Take(3).ToList();

            return res;
        }

        public static Formation GetFormationAtDepth(IEnumerable<Formation> formations, double depth)
        {
            var sortedFormations = formations.OrderBy(x => -x.TvdTope).ToList();
            return sortedFormations.LastOrDefault(x => x.TvdTope <= depth);
        }

        public static double? GetShallowestCementInAnnulus(Annulus annulus)
        {
            if (annulus.InnerBoundary == null) return null;

            List<Tubular> cementedTubularData = annulus.InnerBoundary.OrderBy(x => x.TopeDeCemento).Where(x => x.TopeDeCemento != null).ToList();

            if (cementedTubularData.Count == 0) return null;

            return cementedTubularData.ElementAt(0).TopeDeCemento;
        }

        /// <summary>
        /// Returns the Fracture Gradient based on wether the cement is above of the casing shoe
        /// </summary>
        /// <param name="casingShoeTvd"></param>
        /// <param name="cementTvd"></param>
        /// <param name="fractureGradient"></param>
        /// <returns></returns>
        public static double GetFractureGradientInAnnulus(double casingShoeTvd, double cementTvd, IEnumerable<WellboreGradient> fractureGradient, IEnumerable<Formation> formations)
        {
            if (cementTvd < casingShoeTvd) return 0;

            var formationsList = formations.OrderBy(x => x.MdBase).ToList();
            var fractureGradientList = fractureGradient.OrderBy(x => x.depth_md).ToList();

            double lowestGradient = double.MaxValue;

            List<Formation> belowFormations = formationsList.FindAll(x => x.TvdTope < cementTvd && x.TvdBase > casingShoeTvd);

            if(belowFormations.Count==0 && cementTvd==double.MaxValue)
            {
                belowFormations = new List<Formation> {
                    formationsList.Last()
                };
            }
        
            belowFormations.ForEach(x =>
            {
                double formationGradient = x.GradienteFractura ?? 1.0D;
                lowestGradient = Math.Min(lowestGradient, formationGradient);
            });

            if (lowestGradient == double.MaxValue) lowestGradient = 1;

            return lowestGradient;
        }

        /// <summary>
        /// Returns TVD for a given MD based on Survey Data
        /// </summary>
        /// <param name="survey"></param>
        /// <param name="targetMd"></param>
        /// <returns></returns>
        public static double GetInterpolatedTvd(IEnumerable<SurveyStation> survey, DatumData datum, double targetMd)
        {
            if (targetMd + datum.DatumElevation + datum.AirGap == 0) return 0;
            var surveyData = JsonSerializer.Deserialize<List<SurveyStation>>(JsonSerializer.Serialize(survey.ToList().OrderBy(x => x.Md).ToList()));
            surveyData.ForEach(x =>
            {
                x.Tvd += datum.DatumElevation;
                x.Md += datum.DatumElevation;
            });


            for (int i = 0; i < surveyData.Count(); i++)
            {
                if ((surveyData[i].Md) > targetMd)
                {
                    double m = (double)((surveyData[i].Tvd - surveyData[i - 1].Tvd) / (surveyData[i].Md - surveyData[i - 1].Md));
                    double b = (double)(surveyData[i].Tvd - m * surveyData[i].Md);

                    return m * targetMd + b;
                }
            }

            return 0;
        }


        /// <summary>
        /// Returns the minimum Fracture Gradient Between a Perf Depths
        /// </summary>
        /// <param name="perforation"></param>
        /// <param name="fractureGradient"></param>
        /// <param name="formations"></param>
        /// <returns></returns>
        public static double GetMinimumFractureGradientInPerforations(IEnumerable<SurveyStation> survey, 
            DatumData datum, 
            Perforation perforation, 
            IEnumerable<WellboreGradient> fractureGradient, 
            IEnumerable<Formation> formationsList)
        {
            double topTvd = GetInterpolatedTvd(survey, datum, perforation.StartMD + datum.DatumElevation);
            double bottomTvd = GetInterpolatedTvd(survey, datum, perforation.EndMD + datum.DatumElevation);

            double lowestGradient = double.MaxValue;
            formationsList.ToList().FindAll(x => x.TvdTope < bottomTvd && x.TvdBase > topTvd).ForEach(x =>
            {
                double gradientvalue = x.GradienteFractura ?? 1.0D;
                lowestGradient = Math.Min(lowestGradient, gradientvalue);
            });

            return lowestGradient;
        }

        public static double GetMinimumPressureGradientInPerforations(IEnumerable<SurveyStation> survey,
            DatumData datum,
            Perforation perforation,
            IEnumerable<WellboreGradient> pressureGradient,
            IEnumerable<Formation> formationsList)
        {
            double topTvd = GetInterpolatedTvd(survey, datum, perforation.StartMD + datum.DatumElevation);
            double bottomTvd = GetInterpolatedTvd(survey, datum, perforation.EndMD + datum.DatumElevation);

            double lowestGradient = double.MaxValue;
            formationsList.ToList().FindAll(x => x.TvdTope < bottomTvd && x.TvdBase > topTvd).ForEach(x =>
            {
                var formationGradient = pressureGradient.FirstOrDefault(pressGradElem => pressGradElem.formationname.ToLower() == x.Formacion.ToLower(), new WellboreGradient { value = 0.433*topTvd });
                double gradientvalue = 0.433 * topTvd;
                if (formationGradient != null) gradientvalue = formationGradient.value;
                lowestGradient = Math.Min(lowestGradient, gradientvalue);
            });

            return lowestGradient;
        }
    }
}
