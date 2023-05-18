using System;
using CauldronCodebase;
using CauldronCodebase.GameStates;
using Cysharp.Threading.Tasks;
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
    private EndingsProvider endingsProvider;

    private bool effectPlaying;

    [Inject] private void Construct(DataPersistenceManager dataPersistenceManager,
                                    SoundManager soundManager,
                                    EndingScreen endingScreen,
                                    GameStateMachine gameStateMachine,
                                    EndingsProvider endingsProvider)
    {
        this.dataPersistenceManager = dataPersistenceManager;
        this.soundManager = soundManager;
        this.endingScreen = endingScreen;
        this.gameStateMachine = gameStateMachine;
        this.endingsProvider = endingsProvider;
    }

    public async UniTask ShowStartGameFX()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main_desktop"));

        var start = Instantiate(startGameFX);
        StartGameFX gameFX = start.GetComponentInChildren<StartGameFX>();
        gameFX.soundManager = soundManager;
        //gameFX.PlaySound();
        gameFX.OnEnd += GameFXOnOnEnd;
        effectPlaying = true;
        await UniTask.WaitUntil(() => !effectPlaying);
    }
    
    public void ShowEndGameFX()
    {
        var end = Instantiate(endGameFX).GetComponentInChildren<EndGameFX>();
        end.SoundManager = soundManager;
        end.EndingScreen = endingScreen;
        end.GameStateMachine = gameStateMachine;
        end.EndingsProvider = endingsProvider;
    }

    private void GameFXOnOnEnd()
    {
        effectPlaying = false;
    }
    
    
    public void ShowDayChange(bool isSunrise)
    {
        Instantiate(isSunrise ? sunFX : moonFX);
    }

    public async UniTaskVoid ShowWithDelay(bool isSunrise)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.7));
        
        ShowDayChange(isSunrise);
    }
}
