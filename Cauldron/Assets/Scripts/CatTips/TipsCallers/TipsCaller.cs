using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase.CatTips
{
    public class TipsCaller : MonoBehaviour
    {
        protected CatTipsValidator CatTipsValidator;
        protected CatTipsProvider catTipsProvider;
        protected GameDataHandler gameDataHandler;
        protected MainSettings settings;

        private GameStateMachine gameStateMachine;

        [Inject]
        private void Construct(CatTipsValidator tipsValidator, GameDataHandler dataHandler,
                                MainSettings mainSettings, GameStateMachine stateMachine,
                                CatTipsProvider tipsProvider)
        {
            CatTipsValidator = tipsValidator;
            gameDataHandler = dataHandler;
            settings = mainSettings;
            gameStateMachine = stateMachine;
            catTipsProvider = tipsProvider;
        }

        protected virtual void Start()
        {
            gameStateMachine.OnChangeState += CallTips;
        }

        protected virtual void OnDestroy()
        {
            gameStateMachine.OnChangeState -= CallTips;
        }

        protected virtual void CallTips(GameStateMachine.GamePhase gamePhase)
        {
            if (gamePhase != GameStateMachine.GamePhase.Visitor || gameDataHandler.currentCard.villager.name == "Cat")
            {
                StopAllCoroutines();
            }
        }
    }
}