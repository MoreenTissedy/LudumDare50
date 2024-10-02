using System.Collections.Generic;
using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class GameModeFactory
    {
        private readonly GameStateMachine gameStateMachine;
        private readonly GameDataHandler gameDataHandler;
        private readonly DiContainer container;
        private List<GameModeBase> appliedGameModes;

        [Inject]
        public GameModeFactory(GameStateMachine gameStateMachine, GameDataHandler gameDataHandler, DiContainer container)
        {
            gameStateMachine.OnGameStarted += TryReapplyGameMode;
            gameStateMachine.OnChangeState += CheckGameState;
            this.gameStateMachine = gameStateMachine;
            this.gameDataHandler = gameDataHandler;
            this.container = container;
        }

        private void TryReapplyGameMode()
        {
            if (!gameDataHandler.IsWardrobeUnlocked) return;
            if (gameDataHandler.currentSkin.GameMode is null || !gameDataHandler.currentSkin.GameMode.ShouldReapply) return;
            TryApplyGameMode(gameDataHandler.currentSkin);
        }

        private void CheckGameState(GameStateMachine.GamePhase phase)
        {
            if (phase != GameStateMachine.GamePhase.VisitorWaiting)
            {
                return;
            }
            if (!gameDataHandler.ShouldApplyGameMode())
            {
                return;
            }
            gameStateMachine.OnChangeState -= CheckGameState;
            
            if (gameDataHandler.IsWardrobeUnlocked)
            {
                TryApplyGameMode(gameDataHandler.currentSkin);
            }
        }

        private void TryApplyGameMode(SkinSO skin)
        {
            GameModeBase gameMode = skin.GameMode;
            if (!gameMode)
            {
                return;
            }
            
            container.Inject(gameMode);
            
            if (appliedGameModes is null)
            {
                appliedGameModes = new List<GameModeBase>();
            }
            appliedGameModes.Add(gameMode);
            
            gameMode.Apply();
            Debug.Log("Apply game mode "+gameMode.name);
        }
    }
}