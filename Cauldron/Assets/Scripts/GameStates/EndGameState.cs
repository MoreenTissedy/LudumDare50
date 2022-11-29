using UnityEngine;
using UnityEngine.SceneManagement;

namespace CauldronCodebase.GameStates
{
    public class EndGameState : BaseGameState
    {
        private EndingsProvider.Unlocks _ending;
        
        private EndingScreen _endingScreen;

        public EndGameState(EndingScreen endingScreen)
        {
            _endingScreen = endingScreen;
        }
        public override void Enter()
        {
            _endingScreen.OpenBookWithEnding(_ending);
            _endingScreen.OnClose += ReloadGame;
        }

        public override void Exit()
        {
            Debug.Log("reload scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        public void SetEnding(EndingsProvider.Unlocks ending)
        {
            _ending = ending;
        }

        private void ReloadGame()
        {
            _endingScreen.OnClose -= ReloadGame;
            Exit();
        }
    }
}