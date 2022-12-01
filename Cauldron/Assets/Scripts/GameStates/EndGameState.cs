using UnityEngine;
using UnityEngine.SceneManagement;

namespace CauldronCodebase.GameStates
{
    public class EndGameState : BaseGameState
    {
        private GameStateMachine gameStateMachine;
        
        private EndingScreen _endingScreen;

        public EndGameState(EndingScreen endingScreen,
                            GameStateMachine stateMachine)
        {
            _endingScreen = endingScreen;
            gameStateMachine = stateMachine;
        }
        public override void Enter()
        {
            _endingScreen.OpenBookWithEnding(gameStateMachine.currentEnding);
            _endingScreen.OnClose += ReloadGame;
        }

        public override void Exit()
        {

        }

        private void ReloadGame()
        {
            _endingScreen.OnClose -= ReloadGame;
            Debug.Log("reload scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }
    }
}