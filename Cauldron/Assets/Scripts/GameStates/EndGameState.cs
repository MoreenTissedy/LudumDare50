using Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CauldronCodebase.GameStates
{
    public class EndGameState : BaseGameState
    {
        private GameStateMachine gameStateMachine;
        
        private EndingScreen _endingScreen;

        private DataPersistenceManager dataPersistenceManager;

        private GameFXManager gameFXManager;

        public EndGameState(EndingScreen endingScreen,
                            GameStateMachine stateMachine,
                            DataPersistenceManager persistenceManager,
                            GameFXManager fxManager)
        {
            _endingScreen = endingScreen;
            gameStateMachine = stateMachine;
            dataPersistenceManager = persistenceManager;
            gameFXManager = fxManager;
        }
        public override void Enter()
        {
            gameFXManager.ShowEndGameFX();
            //_endingScreen.OpenBookWithEnding(gameStateMachine.currentEnding);
            _endingScreen.OnClose += ReloadGame;
        }

        public override void Exit()
        {
            
            if (_endingScreen.isActiveAndEnabled)
            {
                _endingScreen.CloseBook();
            }
        }

        private void ReloadGame()
        {
            Debug.Log("reload scene");
            _endingScreen.OnClose -= ReloadGame;
            dataPersistenceManager.NewGame();
            SceneManager.LoadScene(1);
        }
    }
}