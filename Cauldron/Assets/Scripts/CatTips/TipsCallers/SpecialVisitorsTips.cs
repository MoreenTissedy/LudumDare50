using System.Collections;
using CauldronCodebase.GameStates;
using Save;
using UnityEngine;
using Zenject;

namespace CauldronCodebase.CatTips
{
    public class SpecialVisitorsTips : TipsCaller, IDataPersistence
    {
        [Inject] private DataPersistenceManager dataPersistenceManager;
        
        private bool DarkStrangerCame, WitchCame, InquisitorCame;

        protected override void Start()
        {
            dataPersistenceManager.AddToDataPersistenceObjList(this);
            
            base.Start();
        }

        protected override void CallTips(GameStateMachine.GamePhase gamePhase)
        {
            base.CallTips(gamePhase);

            StartCoroutine(CheckSpecialVisitors());
        }

        private IEnumerator CheckSpecialVisitors()
        {
            yield return new WaitForSeconds(settings.catTips.VisitorCheckDelay);
            
            switch (gameDataHandler.currentCard.villager.name)
            {
                case "Inquisition":
                    if (InquisitorCame) yield break;
                    
                    CatTipsValidator.ShowTips(CatTipsGenerator.CreateRandomTips(catTipsProvider.InquisitionTips));
                    InquisitorCame = true;
                    yield break;
                
                case "DarkStranger":
                    if (DarkStrangerCame) yield break;
                    
                    CatTipsValidator.ShowTips(CatTipsGenerator.CreateRandomTips(catTipsProvider.DarkStrangerTips));
                    DarkStrangerCame = true;
                    yield break;
            
                case "WitchMemory":
                    if (int.TryParse(gameDataHandler.currentCard.name[gameDataHandler.currentCard.name.Length-1].ToString(), out int index))
                    {
                        CatTipsValidator.ShowTips(CatTipsGenerator.CreateSequencedTips(catTipsProvider.WitchMemoryTips, index-1));
                        
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