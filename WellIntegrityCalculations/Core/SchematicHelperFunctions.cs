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
        public static IEnumerable<Annulus> GetAnnulusContents(IEnumerable<Tubular> casingData)
        {
            List<Annulus> res = new List<Annulus>();
            bool syntethicTubingAdded = false;

            //It there's no Tubing, we should add a temporary 
            if (casingData.FirstOrDefault(x => x.SectType == "TUBING") == null)
            {
                casingData = casingData.Prepend(new Tubular { AssemblyName = "Synthetic Tubing", SectType = "TUBING", Profundidad = -1 }).ToList();
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
                    } while (tempElem.TopeDeCasing > 0);
                }

                //Find Outer Boundary
                do
                {
                    tempElem = casingDataQueue.Dequeue();
                    tempAnnulus.OuterBoundary.Add(tempElem);
                } while (tempElem.TopeDeCasing > 0);

                res.Add(tempAnnulus);
                annulusIndex++;
                tempAnnulus = new Annulus { index = annulusIndex, InnerBoundary = tempAnnulus.OuterBoundary, OuterBoundary = new List<Tubular>(), Anular = "Anular " + ALPHABET[annulusIndex - 1] };

            }
            if (syntethicTubingAdded)
            {
                res[0].InnerBoundary = new List<Tubular>();
            }

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

            List<Tubular> cementedTubularData = annulus.InnerBoundary.OrderBy(x => -x.TopeDeCemento).Where(x => x.TopeDeCemento != null).ToList();

            if (cementedTubularData.Count == 0) return null;

            return cementedTubularData.ElementAt(0).TopeDeCemento;
        }
    }
}
