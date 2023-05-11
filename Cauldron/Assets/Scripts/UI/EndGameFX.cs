using CauldronCodebase;
using CauldronCodebase.GameStates;
using UnityEngine;

public class EndGameFX : MonoBehaviour
{
    [HideInInspector]public SoundManager SoundManager;
    [HideInInspector]public EndingScreen EndingScreen;
    [HideInInspector]public GameStateMachine GameStateMachine;
    

    public void PlaySound()
    {
        SoundManager.Play(Sounds.GameEnd);
    }

    public void ShowEndingScreen()
    {
        EndingScreen.OpenBookWithEnding(GameStateMachine.currentEnding);
    }
}
