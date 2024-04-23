using System.Collections;
using UnityEngine;

namespace CauldronCodebase.CatTips
{
    public class CheckScaleTips : EncounterTipsCaller
    {
        protected override bool TipShown { get; set; }
        protected override IEnumerator CallTips()
        {
            yield return new WaitForSeconds(settings.catTips.ScaleCheckDelay);
            
            var fame = gameDataHandler.Get(Statustype.Fame);
            var fear = gameDataHandler.Get(Statustype.Fear);
        
            CheckVisitor(gameDataHandler.currentCard.primaryInfluence, gameDataHandler.currentCard.primaryCoef);
            CheckVisitor(gameDataHandler.currentCard.secondaryInfluence, gameDataHandler.currentCard.secondaryCoef);

            void CheckVisitor(Statustype status, float influenceCoef)
            {
                bool influenceIsPositive = influenceCoef > 0;
                switch (status)
                {
                    case Statustype.Fame:
                        if (fame >= gameDataHandler.GetThreshold(Statustype.Fame, true))
                        {
                            CreateTip(catTipsProvider.HighFameTips, true, influenceIsPositive);
                        }

                        if (fame <= gameDataHandler.GetThreshold(Statustype.Fame, false))
                        {
                            CreateTip(catTipsProvider.LowFameTips, false, influenceIsPositive);
                        }
                        break;
                    case Statustype.Fear:
                        if (fear >= gameDataHandler.GetThreshold(Statustype.Fear, true))
                        {
                            CreateTip(catTipsProvider.HighFearTips, true, influenceIsPositive);
                        }

                        if (fear <= gameDataHandler.GetThreshold(Statustype.Fear, false))
                        {
                            CreateTip(catTipsProvider.LowFearTips, false, influenceIsPositive);
                        }
                        break;
                }
            }

            void CreateTip(CatTipsTextSO scaleText, bool high, bool influenceIsPositive)
            {
                TipShown = catTipsValidator.ShowTips(influenceIsPositive ?
                     CatTipsGenerator.CreateTips(scaleText, high ? catTipsProvider.ScaleDOWNTips : catTipsProvider.ScaleUPTips) :
                     CatTipsGenerator.CreateTips(scaleText, high ? catTipsProvider.ScaleUPTips : catTipsProvider.ScaleDOWNTips));
            
            }
        }
    }
}