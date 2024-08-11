using System.Collections;
using UnityEngine;
using Zenject;

namespace CauldronCodebase.CatTips
{
    public class SpecialVisitorsTips : EncounterTipsCaller, IDataPersistence
    {
        [Inject] private DataPersistenceManager dataPersistenceManager;
        
        private bool DarkStrangerCame, WitchCame, InquisitorCame;
        protected override bool TipShown { get; set; }

        protected override void Start()
        {
            dataPersistenceManager.AddToDataPersistenceObjList(this);
            
            base.Start();
        }

        protected override IEnumerator CallTips()
        {
            if(!EncounterIdents.GetAllSpecialCharacters().Contains(gameDataHandler.currentCard.villager.name)) yield break;
            
            yield return new WaitForSeconds(settings.catTips.VisitorCheckDelay);
            
            switch (gameDataHandler.currentCard.villager.name)
            {
                case EncounterIdents.INQUISITION:
                    if (InquisitorCame) yield break;
                    
                    TipShown = catTipsValidator.ShowTips(CatTipsGenerator.CreateRandomTips(catTipsProvider.InquisitionTips));
                    InquisitorCame = true;
                    yield break;
                
                case EncounterIdents.DARK_STRANGER:
                    if (DarkStrangerCame) yield break;
                    
                    TipShown = catTipsValidator.ShowTips(CatTipsGenerator.CreateRandomTips(catTipsProvider.DarkStrangerTips));
                    DarkStrangerCame = true;
                    yield break;
            
                case EncounterIdents.WITCH_MEMORY:
                    if (int.TryParse(gameDataHandler.currentCard.name[gameDataHandler.currentCard.name.Length-1].ToString(), out int index))
                    {
                        TipShown = catTipsValidator.ShowTips(CatTipsGenerator.CreateSequencedTips(catTipsProvider.WitchMemoryTips, index-1));
                    }
                    yield break;
            }
        }

        public void LoadData(GameData data, bool newGame)
        {
            DarkStrangerCame = data.DarkStrangerCame;
            WitchCame = data.WitchCame;
            InquisitorCame = data.InquisitorCame;
        }

        public void SaveData(ref GameData data)
        {
            data.DarkStrangerCame = DarkStrangerCame;
            data.WitchCame = WitchCame;
            data.InquisitorCame = InquisitorCame;
        }
    }
}