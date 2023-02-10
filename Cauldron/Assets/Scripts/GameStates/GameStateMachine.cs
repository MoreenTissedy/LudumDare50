using System.Collections.Generic;
using System;
using UnityEngine;
using Zenject;

namespace CauldronCodebase.GameStates
{
    public class GameStateMachine : MonoBehaviour
    {
        public enum GamePhase
        {
            VisitorWaiting,
            Visitor,
            Night,
            EndGame
        }

        [HideInInspector] public EndingsProvider.Unlocks currentEnding = EndingsProvider.Unlocks.None;
        [HideInInspector] public GamePhase currentGamePhase = GamePhase.VisitorWaiting;

        private Dictionary<GamePhase, BaseGameState> gameStates = new Dictionary<GamePhase, BaseGameState>();

        private BaseGameState _currentGameState;

        public event Action<GamePhase> OnChangeState;


        [Inject]
        public void Construct(StateFactory factory)
        {
            gameStates.Add(GamePhase.VisitorWaiting, factory.CreateVisitorWaitingState());
            gameStates.Add(GamePhase.Visitor, factory.CreateVisitorState());
            gameStates.Add(GamePhase.Night, factory.CreateNightState());
            gameStates.Add(GamePhase.EndGame, factory.CreateEndGameState());
            
            _currentGameState = gameStates[GamePhase.VisitorWaiting];
        }

        private void Start()
        {
            if(GameLoader.IsMenuLoaded()) return;
            _currentGameState.Enter();
        }
 
        public void SwitchState(GamePhase phase)
        {
            _currentGameState.Exit();
            currentGamePhase = phase;
            _currentGameState = gameStates[phase];
            _currentGameState.Enter();
            OnChangeState?.Invoke(phase);

        }
    }
}
