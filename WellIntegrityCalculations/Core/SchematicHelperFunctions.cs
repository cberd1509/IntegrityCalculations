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
        /// From an specific Annulus, finds the inner weakest element (Normally the one with the highest diameter) and returns it
        /// </summary>
        /// <param name="a"></param>
        /// <returns>Element Data</returns>
        public static CasingData GetInnerWeakestElementFromAnnulus(Annulus a)
        {
            return a.InnerBoundary.OrderBy(x => x.CollapsePressure).ElementAt(0);
        }

        /// <summary>
        /// From an specific Annulus, finds the weakest element (Normally the one with the highest diameter) and returns it
        /// </summary>
        /// <param name="a">Annulus</param>
        /// <returns>CasingData of the Weakest Element in Annulus</returns>
        public static CasingData GetOuterWeakestElementFromAnnulus(Annulus a)
        {
            return a.OuterBoundary.OrderBy(x => x.CollapsePressure).ElementAt(0);
        }

        /// <summary>
        /// Returns the list of Annulus and it's contents on each boundary
        /// </summary>
        /// <param name="casingData">Casing and tubular information of the Well Schematic</param>
        /// <returns>Annulus contents</returns>
        public static List<Annulus> GetAnnulusContents(List<CasingData> casingData)
        {
            List<Annulus> res = new List<Annulus>();
            casingData = casingData.OrderBy(item => item.Diameter).ToList();

            bool syntethicTubingAdded = false;

            //It there's no Tubing, we should add a temporary 
            if (casingData.Find(x => x.SectType == CasingSectionType.TUBING) == null)
            {
                casingData = casingData.Prepend(new CasingData(CasingSectionType.TUBING, 0, double.MaxValue, -1)).ToList();
                syntethicTubingAdded = true;
            }

            Queue<CasingData> casingDataQueue = new Queue<CasingData>(casingData);

            int annulusIndex = 0;
            Annulus? tempAnnulus = new Annulus(annulusIndex);
            CasingData? tempElem = null;

            while (casingDataQueue.Count > 0)
            {
                if (tempAnnulus.InnerBoundary.Count == 0)
                {
                    //Find Inner Boundary
                    do
                    {
                        tempElem = casingDataQueue.Dequeue();
                        tempAnnulus.InnerBoundary.Add(tempElem);
                    } while (tempElem.MdTop != 0);
                }

                //Find Outer Boundary
                do
                {
                    tempElem = casingDataQueue.Dequeue();
                    tempAnnulus.OuterBoundary.Add(tempElem);
                } while (tempElem.MdTop != 0);

                res.Add(tempAnnulus);
                annulusIndex++;
                tempAnnulus = new Annulus(annulusIndex, tempAnnulus.OuterBoundary, new List<CasingData>());
            }

            if (syntethicTubingAdded)
            {
                res[0].InnerBoundary = new List<CasingData>();
            }

            return res;
        }

        /// <summary>
        /// Returns a gradient in the form of unit/depth based on the gradient values which must be in the Depth - Absolute value format
        /// P.E: Depth: 3000ft -  Value: 1500 psi => Returns 0.5 psi/ft
        /// </summary>
        /// <param name="targetDepth">Target Depth to get the gradient</param>
        /// <param name="gradientList">List of gradient values in absolute units</param>
        /// <param name="surfaceValue">Value at surface (Depth = 0)</param>
        /// <returns>Value of the gradient in unit/depth</returns>
        /// <exception cref="Exception"></exception>
        public static double GetGradientValueAtDepth(double targetDepth, List<DepthGradient> gradientList, double surfaceValue)
        {
            if (gradientList.Count == 0) throw new Exception("Gradient List is Empty");

            if (gradientList.ElementAt(0).Depth != 0)
            {
                gradientList.Prepend(new DepthGradient { Depth = 0, Value = surfaceValue });
            }

            DepthGradient? upperGradient = null;
            DepthGradient? lowerGradient = null;

            for (int i = 1; i < gradientList.Count; i++) {

                upperGradient = gradientList.ElementAt(i);
                lowerGradient = gradientList.ElementAt(i - 1);

                if (gradientList.ElementAt(i).Depth >= targetDepth)
                {
                    break;    
                }
            }

            return (upperGradient!.Value - lowerGradient!.Value) / (upperGradient.Depth - lowerGradient.Depth);
        }

        /// <summary>
        /// For a given annulus, returns the CementJob associated to one of the Outer casings/liner. If there's no cement, null is returned
        /// </summary>
        /// <param name="annulus"></param>
        /// <param name="cementJobs"></param>
        /// <returns>CementJob when there's a Cement Job associated with the Annulus, null when there's no cement in annulus</returns>
        public static CementJob? GetAnnulusCementJob(Annulus annulus, List<CementJob> cementJobs)
        {
            Dictionary<string, CementJob> cementJobDictionary = new Dictionary<string, CementJob>();
            foreach (CementJob cementJob in cementJobs)
            {
                cementJobDictionary.Add(cementJob.CasingId, cementJob);
            }

            foreach(CasingData casingData in annulus.OuterBoundary)
            {
                if (cementJobDictionary.ContainsKey(casingData.CasingId)) return cementJobDictionary[casingData.CasingId];
            }
            return null;
        }

    }
}
