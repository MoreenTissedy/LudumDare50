using UnityEngine;

namespace CauldronCodebase.GameStates
{
    public class NightState : BaseGameState
    {
        private readonly GameData gameData;
        private readonly MainSettings settings;
        private readonly NightEventProvider nightEvents;
        private readonly EncounterDeckBase cardDeck;
        private readonly NightPanel nightPanel;
        private readonly GameStateMachine stateMachine;

        private readonly StatusChecker statusChecker;
        private readonly EventResolver eventResolver;

        public NightState(GameData gameData,
                          MainSettings settings,
                          NightEventProvider nightEvents,
                          EncounterDeckBase cardDeck,
                          NightPanel nightPanel,
                          GameStateMachine stateMachine)
        {
            this.gameData = gameData;
            this.settings = settings;
            this.nightEvents = nightEvents;
            this.cardDeck = cardDeck;
            this.nightPanel = nightPanel;
            this.stateMachine = stateMachine;

            statusChecker = new StatusChecker(settings, gameData);
            eventResolver = new EventResolver(settings, gameData);
        }
        
        public override void Enter()
        {
            if (IsGameEnd()) return;
            gameData.CalculatePotionsOnLastDays();
            var events = nightEvents.GetEvents(gameData);            
            nightPanel.OpenBookWithEvents(events);
            nightPanel.OnClose += NightPanelOnOnClose;
            foreach (NightEvent nightEvent in events)
            {
                eventResolver.ApplyModifiers(nightEvent);
            }

            cardDeck.NewDayPool(gameData.currentDay);
            cardDeck.DealCards(settings.gameplay.cardsDealtAtNight);
            gameData.currentDay++;
            gameData.cardsDrawnToday = 0;
        }

        private void NightPanelOnOnClose()
        {
            if (IsGameEnd()) return;
            Debug.Log("new day " + gameData.currentDay);
            gameData.NextCountDay();
            stateMachine.SwitchState(GameStateMachine.GamePhase.Visitor);
        }

        private bool IsGameEnd()
        {
            var check = statusChecker.Run();
            if (check != EndingsProvider.Unlocks.None)
            {
                stateMachine.currentEnding = check;
                stateMachine.SwitchState(GameStateMachine.GamePhase.EndGame);
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