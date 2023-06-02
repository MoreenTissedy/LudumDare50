using System.Collections.Generic;
using UnityEngine;

namespace CauldronCodebase.GameStates
{
    public class NightState : BaseGameState
    {
        private readonly GameDataHandler gameDataHandler;
        private readonly MainSettings settings;
        private readonly NightEventProvider nightEvents;
        private readonly EncounterDeck cardDeck;
        private readonly NightPanel nightPanel;
        private readonly GameStateMachine stateMachine;

        private readonly StatusChecker statusChecker;
        private readonly EventResolver eventResolver;
        private readonly List<Encounter> storyCards;
        private readonly RecipeBook recipeBook;
        
        private readonly GameFXManager gameFXManager;

        public NightState(GameDataHandler gameDataHandler,
                          MainSettings settings,
                          NightEventProvider nightEvents,
                          EncounterDeck cardDeck,
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
            storyCards = new List<Encounter>(2);
        }
        
        public override void Enter()
        {
            if (recipeBook.enabled)
            {
                recipeBook.CloseBook();
            }
            EnterWithFX();
        }

        private async void EnterWithFX()
        {
            await gameFXManager.ShowSunset();

            gameDataHandler.CalculatePotionsOnLastDays();
            var events = nightEvents.GetEvents(gameDataHandler);
            nightPanel.OpenBookWithEvents(events);
            nightPanel.OnClose += NightPanelOnOnClose;
            nightPanel.EventClicked += NightPanelOnEventClicked;
        }

        private void NightPanelOnEventClicked(NightEvent nightEvent)
        {
            eventResolver.ApplyModifiers(nightEvent);
            var priorityEvent = eventResolver.AddBonusCards(nightEvent);
            if (priorityEvent)
            {
                storyCards.Add(priorityEvent);
            }
        }

        private async void NightPanelOnOnClose()
        {
            UpdateDeck();
            await gameFXManager.ShowSunrise();
            stateMachine.SwitchState(GameStateMachine.GamePhase.Visitor);
        }

        private void UpdateDeck()
        {
            gameDataHandler.NextDay();
            cardDeck.NewDayPool(gameDataHandler.currentDay);
            
            if (IsGameEnd()) return;
            
            //add priority cards to story card list
            statusChecker.CheckStatusesThreshold();
            
            cardDeck.DealCardsTo(settings.gameplay.targetDeckCount - storyCards.Count);
            cardDeck.ShuffleDeck();
            for (var i = storyCards.Count-1; i >= 0; i--)
            {
                var storyCard = storyCards[i];
                cardDeck.AddToDeck(storyCard, true);
            }
            storyCards.Clear();
            Debug.Log("new day " + gameDataHandler.currentDay);
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
            storyCards.Clear();
            gameFXManager.Clear();
            nightPanel.OnClose -= NightPanelOnOnClose;
            if (nightPanel.IsOpen)
            {
                nightPanel.CloseBook();
            }
        }
    }
}