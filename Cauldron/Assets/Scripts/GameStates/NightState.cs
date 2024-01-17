using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
        private readonly IAchievementManager achievements;
        private readonly EventResolver eventResolver;
        private readonly List<Encounter> storyCards;
        private readonly RecipeBook recipeBook;
        
        private readonly GameFXManager gameFXManager;
        private string storyEnding;

        public NightState(GameDataHandler gameDataHandler,
                          MainSettings settings,
                          NightEventProvider nightEvents,
                          EncounterDeck cardDeck,
                          NightPanel nightPanel,
                          GameStateMachine stateMachine,
                          RecipeBook recipeBook,
                          GameFXManager gameFXManager,
                          StatusChecker statusChecker, 
                          IAchievementManager achievements)
        {
            this.gameDataHandler = gameDataHandler;
            this.settings = settings;
            this.nightEvents = nightEvents;
            this.cardDeck = cardDeck;
            this.nightPanel = nightPanel;
            this.stateMachine = stateMachine;
            this.gameFXManager = gameFXManager;
            this.recipeBook = recipeBook;
            this.statusChecker = statusChecker;
            this.achievements = achievements;

            eventResolver = new EventResolver(settings, gameDataHandler);
            storyCards = new List<Encounter>(2);
        }
        
        public override void Enter()
        {
            if (recipeBook.enabled)
            {
                recipeBook.CloseBook();
            }
            gameDataHandler.SetCurrentCard(null);
            EnterWithFX();
        }

        private async void EnterWithFX()
        {
            await gameFXManager.ShowSunset();

            gameDataHandler.CalculatePotionsOnLastDays();
            var events = nightEvents.GetEvents(gameDataHandler).ToList();
            CheckStoryEnding(events);
            nightPanel.OpenBookWithEvents(events.ToArray());
            nightPanel.EventClicked += NightPanelOnEventClicked;
        }

        private void CheckStoryEnding(List<NightEvent> events)
        {
            for (var index = 0; index < events.Count; index++)
            {
                string tag = events[index].storyTag;
                tag = tag.Split(',')[0];
                if (tag.StartsWith("^"))
                {
                    storyEnding = tag.TrimStart('^').TrimStart('*').Trim();
                    break;
                }
            }
        }

        private void NightPanelOnEventClicked(NightEvent nightEvent)
        {
            nightEvent.OnResolve();
            achievements.TryUnlock(nightEvent);
            eventResolver.ApplyStoryTag(nightEvent);
            eventResolver.ApplyModifiers(nightEvent);
            eventResolver.ApplyFractionShift(nightEvent.fractionData);
            eventResolver.ApplyRecipeHint(nightEvent.recipeHint);
            var priorityEvent = eventResolver.AddBonusCards(nightEvent);
            if (priorityEvent)
            {
                storyCards.Add(priorityEvent);
            }
            if (nightPanel.CurrentPage + 1 >= nightPanel.TotalPages)
            {
                OnAllEventsResolved();
            }
        }

        private async void OnAllEventsResolved()
        {
            if (IsGameEnd()) return;
            UpdateDeck();
            if (cardDeck.NotEnoughCards)
            {
                storyEnding = EndingsProvider.END_DECK;
                await nightPanel.AddEventAsLast(nightEvents.movingEnding);
                nightPanel.NextPage();
            }
            else
            {
                nightPanel.CloseBook();
                await gameFXManager.ShowSunrise();
                stateMachine.SwitchState(GameStateMachine.GamePhase.Visitor);
            }
        }

        private void UpdateDeck()
        {
            gameDataHandler.NextDay();
            
            var priorityCard = statusChecker.CheckStatusesThreshold();
            if (priorityCard)
            {
                storyCards.Add(priorityCard);
            }
            
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
            if (!string.IsNullOrEmpty(storyEnding))
            {
                stateMachine.SwitchToEnding(storyEnding);
                return true;
            }
            var check = statusChecker.Run();
            if (!string.IsNullOrEmpty(check))
            {
                stateMachine.SwitchToEnding(check);
                return true;
            }
            return false;
        }

        public override void Exit()
        {
            storyEnding = string.Empty;
            storyCards.Clear();
            gameFXManager.Clear();
            nightEvents.ClearJoinedEvents();
            nightPanel.EventClicked -= NightPanelOnEventClicked;
            if (nightPanel.IsOpen)
            {
                nightPanel.CloseBook();
            }
        }
    }
}