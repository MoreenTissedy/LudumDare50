using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CauldronCodebase.GameStates
{
    public class VisitorWaitingState : BaseGameState
    {
        private readonly MainSettings _settings;
        private readonly GameDataHandler gameDataHandler;
        private readonly GameStateMachine _stateMachine;
        private readonly GameFXManager _gameFXManager;

        public VisitorWaitingState(MainSettings settings, GameDataHandler gameDataHandler, GameStateMachine stateMachine, GameFXManager fxManager)
        {
            _settings = settings;
            this.gameDataHandler = gameDataHandler;
            _stateMachine = stateMachine;
            _gameFXManager = fxManager;
        }
        
        public override void Enter()
        {
            ExitStateWithDelay();
        }

        public override void Exit()
        {
            //It's not good to have empty implementations, but we'll leave it as is for now
        }

        private async void ExitStateWithDelay()
        {
            await Task.Delay(TimeSpan.FromSeconds(_settings.gameplay.villagerDelay));

            if (gameDataHandler.cardsDrawnToday >= _settings.gameplay.cardsPerDay)
            {
                Debug.Log("switch to night");
                
                _stateMachine.SwitchState(GameStateMachine.GamePhase.Night);
            }
            else
            {
                _stateMachine.SwitchState(GameStateMachine.GamePhase.Visitor);
            }
        }
    }
}