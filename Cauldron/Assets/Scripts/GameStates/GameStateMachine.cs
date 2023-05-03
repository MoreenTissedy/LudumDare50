using System.Collections.Generic;
using System;
using Save;
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

        private readonly Dictionary<GamePhase, BaseGameState> gameStates = new Dictionary<GamePhase, BaseGameState>();

        private BaseGameState currentGameState;

        private DataPersistenceManager dataPersistenceManager;
        private GameDataHandler gameData;

        public event Action<GamePhase> OnChangeState;


        [Inject]
        public void Construct(StateFactory factory, DataPersistenceManager persistenceManager, GameDataHandler gameData)
        {
            gameStates.Add(GamePhase.VisitorWaiting, factory.CreateVisitorWaitingState());
            gameStates.Add(GamePhase.Visitor, factory.CreateVisitorState());
            gameStates.Add(GamePhase.Night, factory.CreateNightState());
            gameStates.Add(GamePhase.EndGame, factory.CreateEndGameState());
            
            dataPersistenceManager = persistenceManager;
            this.gameData = gameData;
        }

        private void Start()
        {
            RunStateMachine();
        }

        public void RunStateMachine()
        {
            dataPersistenceManager.LoadDataPersistenceObj();
            currentGameState = gameStates[gameData.gamePhase];
            
            if (!PlayerPrefs.HasKey("CurrentRound"))
            {
                PlayerPrefs.SetInt("CurrentRound", 0);
                gameData.currentDeck.ForgetCards();
            }
            else
            {
                gameData.currentRound = PlayerPrefs.GetInt("CurrentRound");
            }
            
            currentGameState.Enter();
        }
 
        public void SwitchState(GamePhase phase)
        {
            currentGameState.Exit();
            currentGamePhase = phase;
            currentGameState = gameStates[phase];
            gameData.gamePhase = phase;
            currentGameState.Enter();
            OnChangeState?.Invoke(phase);
            dataPersistenceManager.SaveGame();
        }
    }
}
