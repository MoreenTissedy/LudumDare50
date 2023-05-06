using System;
using System.Collections;
using System.Threading.Tasks;
using CauldronCodebase;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameFXManager : MonoBehaviour
{
    [SerializeField] private GameObject startGameFX, sunFX, moonFX;

    private DataPersistenceManager dataPersistenceManager;
    private SoundManager soundManager;

    [Inject] private void Construct(DataPersistenceManager dataPersistenceManager, SoundManager soundManager)
    {
        this.dataPersistenceManager = dataPersistenceManager;
        this.soundManager = soundManager;
    }
    
    public void ShowStartGameFX()
    {
        if(!dataPersistenceManager.IsNewGame) return;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main_desktop"));

        var start = Instantiate(startGameFX);
        start.GetComponentInChildren<StartGameFX>().soundManager = soundManager;

    }

    public void ShowDayChange(bool isSunrise)
    {
        Instantiate(isSunrise ? sunFX : moonFX);
    }

    public async void ShowWithDelay(bool isSunrise)
    {
        await Task.Delay(TimeSpan.FromSeconds(0.7f));
        ShowDayChange(isSunrise);
    }

}
