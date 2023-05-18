using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CauldronCodebase.GameStates
{
    public class NightState : BaseGameState
    {
        private readonly GameDataHandler gameDataHandler;
        private readonly MainSettings settings;
        private readonly NightEventProvider nightEvents;
        private readonly EncounterDeckBase cardDeck;
        private readonly NightPanel nightPanel;
        private readonly GameStateMachine stateMachine;

        private readonly StatusChecker statusChecker;
        private readonly EventResolver eventResolver;
        private readonly RecipeBook recipeBook;
        
        private readonly GameFXManager gameFXManager;

        public NightState(GameDataHandler gameDataHandler,
                          MainSettings settings,
                          NightEventProvider nightEvents,
                          EncounterDeckBase cardDeck,
                          NightPanel nightPanel,
                          GameStateMachine stateMachine,
                          RecipeBook book,
                          GameFXManager gameFXManager)
        {
            this.gameDataHandler = gameDataHandler;
            this.settings = settings;
            this.nightEvents = nightEvents;
            this.cardDeck = cardDeck;
            this.nightPanel = nightPanel;
            this.stateMachine = stateMachine;
            this.gameFXManager = gameFXManager;
            recipeBook = book;

            statusChecker = new StatusChecker(settings, gameDataHandler);
            eventResolver = new EventResolver(settings, gameDataHandler);
        }
        
        public override void Enter()
        {
            if (recipeBook.enabled)
            {
                recipeBook.CloseBook();
            }
            EnterWithDelay();
        }

        private async void EnterWithDelay()
        {
            await Task.Delay(TimeSpan.FromSeconds(settings.gameplay.nightStartDelay));
            
            if (IsGameEnd()) return;
            gameDataHandler.CalculatePotionsOnLastDays();
            var events = nightEvents.GetEvents(gameDataHandler);
            nightPanel.OpenBookWithEvents(events);
            nightPanel.OnClose += NightPanelOnOnClose;
            gameDataHandler.NextDay();
            cardDeck.NewDayPool(gameDataHandler.currentDay);
            cardDeck.DealCards(settings.gameplay.cardsDealtAtNight);
        }

        private void NightPanelOnOnClose()
        {
            if (IsGameEnd()) return;
            statusChecker.CheckStatusesThreshold();
            cardDeck.AddStoryCards();
            Debug.Log("new day " + gameDataHandler.currentDay);
            gameFXManager.ShowWithDelay(true).Forget();
            stateMachine.SwitchState(GameStateMachine.GamePhase.VisitorWaiting);
        }

        private bool IsGameEnd()
        {
            var check = statusChecker.Run();
            if (check != EndingsProvider.Unlocks.None)
            {
                stateMachine.currentEnding = check;
                stateMachine.SwitchState(GameStateMachine.GamePhase.EndGame);
                gameDataHandler.RememberRound();
                return true;
            }

            return false;
        }

        public override void Exit()
        {            
            nightPanel.OnClose -= NightPanelOnOnClose;
            if (nightPanel.isActiveAndEnabled)
            {
                nightPanel.CloseBook();
            }
        }
    }
}