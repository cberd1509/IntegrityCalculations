using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculations.Services
{
    public class CalculationService : ICalculationService
    {

        const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        ILogger<ICalculationService> _logger;
        public CalculationService(ILogger<ICalculationService> logger)
        {
            _logger = logger;
        }
        public void GetWellMawop(MawopCalculationRequestDTO requestData)
        {
            List<Annulus> annulusList = GetAnnulusContents(requestData.CasingData);
            List<CalculationElement> calculationElements = new List<CalculationElement>(); 

            //Rule #1: Inner Weakest Element in Annulus
            CalculationElement rule1Element = new CalculationElement{ RuleIndex = calculationElements.Count+1, RuleTitle = "Innermost Casing or Tubing (Annulus A)"  };
            if (annulusList.ElementAt(0).InnerBoundary.Count == 0)
            {
                rule1Element.IsRelevant = false;
            }
            else
            {
                CasingData element = GetInnerWeakestElementFromAnnulus(annulusList.ElementAt(0));
                rule1Element.IsRelevant = true;
                rule1Element.Diameter = element.Diameter;
                rule1Element.CollapsePressure = element.CollapsePressure;
                rule1Element.BurstPressure = element.BurstPressure;
                rule1Element.PressureGradient = 0; //TODO: Get Pressure Gradient from Somewhere
            }
            calculationElements.Add(rule1Element);

            //Rule #2: Casing Analysis for Each Annulus
            for(int i = 1; i < annulusList.Count; i++)
            {
                CalculationElement ruleElement = new CalculationElement { RuleIndex = calculationElements.Count + 1, RuleTitle = $"Outermost Casing (Anulus {ALPHABET[i - 1]})" };
                CasingData elem = GetOuterWeakestElementFromAnnulus(annulusList[i]);

                ruleElement.IsRelevant = true;
                ruleElement.Diameter = elem.Diameter;
                ruleElement.CasingShoeTvd = elem.TvdBase;

                if (true == false) //TODO: Implement this
                {
                    ruleElement.TopOfCementInAnular = 0;
                }

                ruleElement.BurstPressure = elem.BurstPressure;
                ruleElement.CollapsePressure = elem.CollapsePressure;
                ruleElement.PressureGradient = 0; //TODO: Implement this
                ruleElement.BelowFormationGradient = 0; //TODO: Implement this

                calculationElements.Add(ruleElement);
            }
           
        }
        public List<Annulus> GetAnnulusContents(List<CasingData> casingData)
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

        public CasingData GetInnerWeakestElementFromAnnulus(Annulus a)
        {
            return a.InnerBoundary.OrderBy(x => x.CollapsePressure).ElementAt(0);
        }

        public CasingData GetOuterWeakestElementFromAnnulus(Annulus a)
        {
            return a.OuterBoundary.OrderBy(x => x.CollapsePressure).ElementAt(0);
        }
    }
}
