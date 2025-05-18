using System.IO;
using Cysharp.Threading.Tasks;
using EasyLoc;
using UnityEngine;
using UnityEngine.Video;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class MainMenu : MonoBehaviour
    {
        public MenuOverlayManager overlayManager;
        public OverlayLayer dummyOverlayLayer;
        public FlexibleButton continueGame;
        public FlexibleButton quit;
        public FlexibleButton newGame;

        [Header("Settings")] public FlexibleButton settings;

        [Header("Authors")] 
        [SerializeField] private FlexibleButton authorsButton;

        [Header("Fade In Out")] [SerializeField] [Tooltip("Fade in seconds")]
        private float fadeNewGameDuration;

        public Canvas menuHud;

        [Inject] private DataPersistenceManager dataPersistenceManager;
        [Inject] private FadeController fadeController;
        [Inject] private SoundManager soundManager;
        [Inject] private LocalizationTool localizationTool;        
        [Inject] private MilestoneProvider milestoneProvider;
        [Inject] private PlayerProgressProvider playerProgressProvider;
        [Inject] private VillagerFamiliarityChecker villagerChecker;
        [Inject] private InputManager inputManager;

        private void Start()
        {
            if (!GameLoader.IsGameLoaded())
            {
                soundManager.SetMusic(Music.Menu, false);
            }
            if (!dataPersistenceManager.IsSaveFound())
            {
                HideContinueButton();
            }
            newGame.gameObject.GetComponent<NewGameButton>().UpdateButton();

            continueGame.OnClick += ContinueClick;
            quit.OnClick += GameLoader.Exit;
            newGame.OnClick += NewGameClick;
            settings.OnClick += overlayManager.OpenSettings;
            authorsButton.OnClick += overlayManager.OpenAuthors;

            inputManager.SetCursor(false);
        }

        public void ResetGameData(bool saveLanguage = true)
        {
            var loadedLanguage = localizationTool.GetSavedLanguage();
            PlayerPrefs.DeleteAll();
            ClearAllSaveFiles();
            dataPersistenceManager.NewGame();
            if (saveLanguage) PlayerPrefs.SetString(PrefKeys.LanguageKey, loadedLanguage.ToString());
            HideContinueButton();
            newGame.gameObject.GetComponent<NewGameButton>().UpdateButton();
            Debug.LogWarning("All data cleared!");
        }

        private void ClearAllSaveFiles()
        {
            string dataDirPath = Application.persistentDataPath;
            string SubFolder = "Saves";
            string subDirPath = Path.Combine(dataDirPath, SubFolder);
            DirectoryInfo di = new DirectoryInfo(subDirPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            milestoneProvider.LoadMilestones();
            playerProgressProvider.Update();
            villagerChecker.Update();
        }

        private void HideContinueButton()
        {
            continueGame.gameObject.SetActive(false);
        }

        private void NewGameClick()
        {
            switch (dataPersistenceManager.IsSaveFound())
            {
                case true:
                    Debug.LogWarning("The saved data has been deleted and a new game has been started");
                    PlayerPrefs.DeleteKey(PrefKeys.UniqueCards);
                    StartNewGame();
                    break;

                case false:
                    StartNewGame();
                    break;
            }
        }

        private void ContinueClick()
        {
            GameLoader.UnloadMenu();
            inputManager.SetCursor(true);
        }

        private async void StartNewGame()
        {
            overlayManager.AddLayer(dummyOverlayLayer);
            await fadeController.FadeIn(duration: fadeNewGameDuration);
            await TryPlayIntroVideo();
            GameLoader.ReloadGame();
            dataPersistenceManager.NewGame();
        }

        private async UniTask TryPlayIntroVideo()
        {
            if (PlayerPrefs.HasKey(PrefKeys.VideoWatched))
            {
                return;
            }

            soundManager.StopMusic();
            menuHud.enabled = false;
            
            
            var video = Instantiate(Resources.Load("Video")) as GameObject;
            var player = video.GetComponentInChildren<VideoPlayer>();
            
            await fadeController.FadeOut(0.3f);
            await UniTask.WaitWhile(() => player.isPlaying);
            await fadeController.FadeIn(0.3f);
            
            PlayerPrefs.SetInt(PrefKeys.VideoWatched, 1);
        }

        
        private void OnDestroy()
        {
            continueGame.OnClick -= ContinueClick;
            quit.OnClick -= GameLoader.Exit;
            newGame.OnClick -= NewGameClick;
            settings.OnClick -= overlayManager.OpenSettings;
            authorsButton.OnClick -= overlayManager.OpenAuthors;          
        }
    }
}