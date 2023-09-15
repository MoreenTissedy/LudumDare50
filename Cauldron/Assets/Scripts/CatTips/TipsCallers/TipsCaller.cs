using CauldronCodebase.GameStates;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace CauldronCodebase.CatTips
{
    public class TipsCaller : MonoBehaviour
    {
        [SerializeField] protected bool TipsWithRecipe;
        [Header("Potion unlock settings")] 
        [ShowIf("TipsWithRecipe")]
        [SerializeField] protected int WrongPotionThreshold = 3;
        [ShowIf("TipsWithRecipe")]
        [SerializeField][Range(0, 100)] protected float ChanceToUnlock = 70;
        
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
            gameStateMachine.OnChangeState += TryCallTips;
        }

        protected virtual void OnDestroy()
        {
            gameStateMachine.OnChangeState -= TryCallTips;
        }

        protected virtual void CallTips()
        {
        }

        private void TryCallTips(GameStateMachine.GamePhase gamePhase)
        {
            StopAllCoroutines();
            if (gamePhase == GameStateMachine.GamePhase.Visitor || gameDataHandler.currentCard.villager.name != "Cat")
            {
                CallTips();
            }
        }
    }
}