using System;
using System.Collections;
using System.Threading.Tasks;
using CauldronCodebase;
using CauldronCodebase.GameStates;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameFXManager : MonoBehaviour
{
    [SerializeField] private GameObject startGameFX, endGameFX;
    [SerializeField] private DayStageFX sunFX, moonFX;

    private DataPersistenceManager dataPersistenceManager;
    private SoundManager soundManager;
    private EndingScreen endingScreen;
    private GameStateMachine gameStateMachine;

    [Inject] private void Construct(DataPersistenceManager dataPersistenceManager,
                                    SoundManager soundManager,
                                    EndingScreen endingScreen,
                                    GameStateMachine gameStateMachine)
    {
        this.dataPersistenceManager = dataPersistenceManager;
        this.soundManager = soundManager;
        this.endingScreen = endingScreen;
        this.gameStateMachine = gameStateMachine;
    }
    
    public void ShowStartGameFX()
    {
        if(!dataPersistenceManager.IsNewGame) return;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main_desktop"));

        var start = Instantiate(startGameFX).GetComponentInChildren<StartGameFX>();
        start.SoundManager = soundManager;

    }

    public void ShowEndGameFX()
    {
        var end = Instantiate(endGameFX).GetComponentInChildren<EndGameFX>();
        end.SoundManager = soundManager;
        end.EndingScreen = endingScreen;
        end.GameStateMachine = gameStateMachine;
    }

    public void ShowDayChange(bool isSunrise)
    {
        var stage = Instantiate(isSunrise ? sunFX : moonFX);
        stage.SoundManager = soundManager;
    }

    public async void ShowWithDelay(bool isSunrise)
    {
        await Task.Delay(TimeSpan.FromSeconds(0.7f));
        ShowDayChange(isSunrise);
    }

}
