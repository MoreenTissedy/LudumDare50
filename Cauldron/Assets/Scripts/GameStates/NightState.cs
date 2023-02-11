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

        public NightState(GameDataHandler gameDataHandler,
                          MainSettings settings,
                          NightEventProvider nightEvents,
                          EncounterDeckBase cardDeck,
                          NightPanel nightPanel,
                          GameStateMachine stateMachine)
        {
            this.gameDataHandler = gameDataHandler;
            this.settings = settings;
            this.nightEvents = nightEvents;
            this.cardDeck = cardDeck;
            this.nightPanel = nightPanel;
            this.stateMachine = stateMachine;

            statusChecker = new StatusChecker(settings, gameDataHandler);
            eventResolver = new EventResolver(settings, gameDataHandler);
        }
        
        public override void Enter()
        {
            if (IsGameEnd()) return;
            gameDataHandler.CalculatePotionsOnLastDays();
            var events = nightEvents.GetEvents(gameDataHandler);            
            nightPanel.OpenBookWithEvents(events);
            nightPanel.OnClose += NightPanelOnOnClose;
            cardDeck.NewDayPool(gameDataHandler.currentDay);
            cardDeck.DealCards(settings.gameplay.cardsDealtAtNight);
            
        }

        private void NightPanelOnOnClose()
        {
            if (IsGameEnd()) return;
            Debug.Log("new day " + gameDataHandler.currentDay);
            gameDataHandler.NextDay();
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