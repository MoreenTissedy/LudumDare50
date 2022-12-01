using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CauldronCodebase.GameStates
{
    public class VisitorWaitingState : BaseGameState
    {
        private MainSettings _settings;
        private GameData gameData;
        private GameStateMachine _stateMachine;

        public VisitorWaitingState(MainSettings settings, GameData gameData, GameStateMachine stateMachine)
        {
            _settings = settings;
            this.gameData = gameData;
            _stateMachine = stateMachine;
        }
        
        public override void Enter()
        {
            ExitStateWithDelay();
        }

        public override void Exit()
        {
            
        }

        private async Task ExitStateWithDelay()
        {
            await Task.Delay(TimeSpan.FromSeconds(_settings.gameplay.villagerDelay));

            if (gameData.cardsDrawnToday >= _settings.gameplay.cardsPerDay)
            {
                Debug.Log("switch to night");
                _stateMachine.SwitchState(GameStateMachine.GamePhase.Night);
            }
            else
            {
                Debug.Log("switch to Visitor");
                _stateMachine.SwitchState(GameStateMachine.GamePhase.Visitor);
            }
        }
    }
}