using System;
using UnityEngine;
using Zenject;

namespace CauldronCodebase.GameStates
{
    public class NightState : BaseGameState
    {
        private GameData gameData;
        private MainSettings _settings;
        private NightEventProvider _nightEvents;
        private EncounterDeckBase _cardDeck;
        private NightPanel _nightPanel;
        private StateMachine _stateMachine;

        private StatusChecker _statusChecker;

        public event Action<int> NewDay; 
        public NightState(GameData gameData,
                          MainSettings settings,
                          NightEventProvider nightEvents,
                          EncounterDeckBase cardDeck,
                          NightPanel nightPanel,
                          StateMachine stateMachine)
        {
            this.gameData = gameData;
            _settings = settings;
            _nightEvents = nightEvents;
            _cardDeck = cardDeck;
            _nightPanel = nightPanel;
            _stateMachine = stateMachine;

            _statusChecker = new StatusChecker(settings, stateMachine);
        }
        
        public override void Enter()
        {
            NewDay?.Invoke(gameData.currentDay + 1);
            
            var events = _nightEvents.GetEvents(gameData);
            _nightPanel.NightState = this;
            _nightPanel.OpenBookWithEvents(events);
            foreach (NightEvent nightEvent in events)
            {
                nightEvent.ApplyModifiers(gameData, _settings);
            }

            _cardDeck.NewDayPool(gameData.currentDay);
            _cardDeck.DealCards(_settings.gameplay.cardsDealtAtNight);
            gameData.currentDay++;
            gameData.cardsDrawnToday = 0;
        }

        public override void Exit()
        {
            //status checker will end game and then state machine will switch one more time = bug)))
            //the checker should not switch the machine, it should do the calculation and return the result (i.e. ending or None), then we shall switch here according to that result 
            _statusChecker.Run();
            Debug.Log("new day "+gameData.currentDay);
            
            _stateMachine.SwitchState(_stateMachine.VisitorWaitingState, false);
        }

    }
}