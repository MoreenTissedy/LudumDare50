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
        private GameStateMachine _stateMachine;

        private StatusChecker _statusChecker;

        public NightState(GameData gameData,
                          MainSettings settings,
                          NightEventProvider nightEvents,
                          EncounterDeckBase cardDeck,
                          NightPanel nightPanel,
                          GameStateMachine stateMachine)
        {
            this.gameData = gameData;
            _settings = settings;
            _nightEvents = nightEvents;
            _cardDeck = cardDeck;
            _nightPanel = nightPanel;
            _stateMachine = stateMachine;

            _statusChecker = new StatusChecker(settings, stateMachine, gameData);
        }
        
        public override void Enter()
        {          
            var events = _nightEvents.GetEvents(gameData);            
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
            var check = _statusChecker.Run();
            if(check == EndingsProvider.Unlocks.None)
            {
                Debug.Log("new day " + gameData.currentDay);
            }
            else
            {
                _stateMachine.currentEnding = check;
                _stateMachine.SwitchState(GameStateMachine.GamePhase.EndGame);
            }         
        }
    }
}