using Cysharp.Threading.Tasks;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using Universal;

namespace CauldronCodebase.GameStates
{
    public class EndGameState : BaseGameState
    {
        private EndingScreen _endingScreen;
        private DataPersistenceManager dataPersistenceManager;
        private GameFXManager gameFXManager;
        private string currentEnding;

        public EndGameState(EndingScreen endingScreen,
                            DataPersistenceManager persistenceManager,
                            GameFXManager fxManager)
        {
            _endingScreen = endingScreen;
            dataPersistenceManager = persistenceManager;
            gameFXManager = fxManager;
        }
        public override void Enter()
        {
            ShowEffectAndEnding().Forget();
        }

        public void SetEnding(string tag)
        {
            currentEnding = tag;
        }

        private async UniTaskVoid ShowEffectAndEnding()
        {
            await gameFXManager.ShowEndGame(currentEnding);
            _endingScreen.Open(currentEnding);
            _endingScreen.OnClose += ReloadGame;
        }

        public override void Exit()
        {
            gameFXManager.Clear();
            if (_endingScreen.isActiveAndEnabled)
            {
                _endingScreen.Close();
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