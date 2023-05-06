using CauldronCodebase;
using Cysharp.Threading.Tasks;
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

    private bool effectPlaying;

    [Inject] private void Construct(DataPersistenceManager dataPersistenceManager, SoundManager soundManager)
    {
        this.dataPersistenceManager = dataPersistenceManager;
        this.soundManager = soundManager;
    }
    
    public async UniTask ShowStartGameFX()
    {
        if (GameLoader.IsMenuOpen())
        {
            return;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main_desktop"));

        var start = Instantiate(startGameFX);
        StartGameFX gameFX = start.GetComponentInChildren<StartGameFX>();
        gameFX.soundManager = soundManager;
        gameFX.PlaySound();
        gameFX.OnEnd += GameFXOnOnEnd;
        effectPlaying = true;
        await UniTask.WaitUntil(() => !effectPlaying);
    }

    private void GameFXOnOnEnd()
    {
        effectPlaying = false;
    }
}
