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
        
            CheckVisitor(gameDataHandler.currentCard.primaryInfluence);
            CheckVisitor(gameDataHandler.currentCard.secondaryInfluence);

            void CheckVisitor(Statustype status)
            {
                switch (status)
                {
                    case Statustype.Fame:
                        if (fame >= gameDataHandler.GetThreshold(Statustype.Fame, true))
                        {
                            CreateTip(catTipsProvider.HighFameTips, true);
                        }

                        if (fame <= gameDataHandler.GetThreshold(Statustype.Fame, false))
                        {
                            CreateTip(catTipsProvider.LowFameTips, false);
                        }
                        break;
                    case Statustype.Fear:
                        if (fear >= gameDataHandler.GetThreshold(Statustype.Fear, true))
                        {
                            CreateTip(catTipsProvider.HighFearTips, true);
                        }

                        if (fear <= gameDataHandler.GetThreshold(Statustype.Fear, false))
                        {
                            CreateTip(catTipsProvider.LowFearTips, false);
                        }
                        break;
                }
            }

            void CreateTip(CatTipsTextSO scaleText, bool high)
            {
                TipShown = catTipsValidator.ShowTips(gameDataHandler.currentCard.primaryCoef > 0
                    ? CatTipsGenerator.CreateTips(scaleText, high ? catTipsProvider.ScaleDOWNTips : catTipsProvider.ScaleUPTips)
                    : CatTipsGenerator.CreateTips(scaleText, high ? catTipsProvider.ScaleUPTips : catTipsProvider.ScaleDOWNTips));
            
            }
        }
    }
}