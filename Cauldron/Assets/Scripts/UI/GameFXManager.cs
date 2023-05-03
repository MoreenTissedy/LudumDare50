using CauldronCodebase;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameFXManager : MonoBehaviour
{
    [SerializeField] private GameObject startGameFX;
    [SerializeField] private Camera uiCamera;

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
}
