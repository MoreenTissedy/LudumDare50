using Cysharp.Threading.Tasks;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CauldronCodebase.GameStates
{
    public class EndGameState : BaseGameState
    {
        private EndingScreen endingScreen;
        private DataPersistenceManager dataPersistenceManager;
        private GameFXManager gameFXManager;
        private string currentEnding;

        public EndGameState(DataPersistenceManager persistenceManager,
                            GameFXManager fxManager)
        {
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
            var fx = gameFXManager.ShowEndGame(currentEnding);
            var loading = Resources.LoadAsync<EndingScreen>(ResourceIdents.EndingScreen);
            await UniTask.WhenAll(fx, loading.ToUniTask());
            endingScreen = Object.Instantiate(loading.asset) as EndingScreen;
            endingScreen.Open(currentEnding);
            endingScreen.OnClose += ReloadGame;
        }

        public override void Exit()
        {
            gameFXManager.Clear();
            if (endingScreen != null && endingScreen.isActiveAndEnabled)
            {
                endingScreen.Close();
            }
        }

        private void ReloadGame()
        {
            Debug.Log("reload scene");
            endingScreen.OnClose -= ReloadGame;
            dataPersistenceManager.NewGame();
            SceneManager.LoadScene(1);
        }
    }
}