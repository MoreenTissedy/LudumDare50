using System.Collections;
using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase.CatTips
{
    public abstract class EncounterTipsCaller : MonoBehaviour
    {
        protected CatTipsValidator catTipsValidator;
        protected CatTipsProvider catTipsProvider;
        protected GameDataHandler gameDataHandler;
        protected MainSettings settings;

        private GameStateMachine gameStateMachine;
        protected abstract bool TipShown { get; set; }

        [Inject]
        private void Construct(CatTipsValidator tipsValidator, GameDataHandler dataHandler,
                                MainSettings mainSettings, GameStateMachine stateMachine,
                                CatTipsProvider tipsProvider)
        {
            catTipsValidator = tipsValidator;
            gameDataHandler = dataHandler;
            settings = mainSettings;
            gameStateMachine = stateMachine;
            catTipsProvider = tipsProvider;
        }

        protected virtual void Start()
        {
            gameStateMachine.OnChangeState += TryCallTips;
        }

        protected virtual void OnDestroy()
        {
            gameStateMachine.OnChangeState -= TryCallTips;
        }

        private void TryCallTips(GameStateMachine.GamePhase gamePhase)
        {
            StopAllCoroutines();
            if (gamePhase == GameStateMachine.GamePhase.Visitor || gameDataHandler.currentCard.villager.name != EncounterIdents.CAT)
            {
                StartCoroutine(CallTips());
            }
            else if (TipShown)
            {
                catTipsValidator.HideTips();
                TipShown= false;
            }
        }

        protected abstract IEnumerator CallTips();
    }
}