using System.Collections.Generic;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        
        [SerializeField] private Transform uiOverlayRoot;
        public GamePhase currentGamePhase = GamePhase.VisitorWaiting;

        private readonly Dictionary<GamePhase, BaseGameState> gameStates = new Dictionary<GamePhase, BaseGameState>();

        private BaseGameState currentGameState;

        private DataPersistenceManager dataPersistenceManager;
        private GameDataHandler gameData;
        private FadeController fadeController;

        private GameFXManager gameFXManager;

        public event Action<GamePhase> OnChangeState;
        public event Action OnGameStarted;
        public event Action OnNewDay;
        
        private CancellationTokenSource cancellationTokenSource;


        [Inject]
        public void Construct(StateFactory factory, DataPersistenceManager persistenceManager, 
            GameDataHandler gameData, GameFXManager fxManager, FadeController fadeController)
        {
            gameStates.Add(GamePhase.VisitorWaiting, factory.CreateVisitorWaitingState());
            gameStates.Add(GamePhase.Visitor, factory.CreateVisitorState());
            gameStates.Add(GamePhase.Night, factory.CreateNightState());
            gameStates.Add(GamePhase.EndGame, factory.CreateEndGameState());
            
            
            dataPersistenceManager = persistenceManager;
            this.gameData = gameData;
            gameFXManager = fxManager;
            this.fadeController = fadeController;
        }

        private void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();

            RunStateMachine(cancellationTokenSource.Token);
        }

        private void OnDestroy()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        public async void RunStateMachine(CancellationToken cancellationToken)
        {
            dataPersistenceManager.LoadDataPersistenceObj();
            currentGamePhase = gameData.gamePhase;
            currentGameState = gameStates[gameData.gamePhase];
            if (dataPersistenceManager.IsNewGame)
            {
                if (GameLoader.IsMenuOpen())
                {
                    return;
                }
                fadeController.FadeOut(0, 1f).Forget();
                await gameFXManager.ShowStartGame();
            }
            else
            {
                await UniTask.NextFrame(cancellationToken); //to avoid injection problems
                while (GameLoader.IsMenuOpen())
                {
                    await UniTask.NextFrame(cancellationToken); 
                }
            }
            OnGameStarted?.Invoke();
            dataPersistenceManager.SaveGame();
            currentGameState.Enter();
        }
 
        public void SwitchState(GamePhase phase)
        {
            var isNewDay = currentGamePhase == GamePhase.Night;
            
            currentGameState.Exit();
            currentGamePhase = phase;
            currentGameState = gameStates[phase];
            gameData.gamePhase = phase;
            currentGameState.Enter();
            OnChangeState?.Invoke(phase);
            if (isNewDay) OnNewDay?.Invoke();
            dataPersistenceManager.SaveGame();
        }

        public void SwitchToEnding(string tag)
        {
            var night = gameStates[GamePhase.EndGame] as EndGameState;
            night?.SetEnding(tag, uiOverlayRoot);
            gameData.RememberRound();
            SwitchState(GamePhase.EndGame);
        }
    }
}
