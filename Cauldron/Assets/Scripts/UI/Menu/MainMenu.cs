using System.IO;
using System.Threading.Tasks;
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
        public AnimatedButton continueGame;
        public AnimatedButton quit;
        public AnimatedButton newGame;

        [Header("Settings")] public AnimatedButton settings;
        public SettingsMenu settingsMenu;

        [Header("Authors")] 
        [SerializeField] private AnimatedButton authorsButton;
        [SerializeField] private AuthorsMenu authorsMenu;

        [Header("Fade In Out")] [SerializeField] [Tooltip("Fade in seconds")]
        private float fadeNewGameDuration;

        public Canvas menuHud;

        [Inject] private DataPersistenceManager dataPersistenceManager;
        [Inject] private FadeController fadeController;
        [Inject] private SoundManager soundManager;
        [Inject] private LocalizationTool localizationTool;        
        [Inject] private MilestoneProvider milestoneProvider;
        [Inject] private PlayerProgressProvider playerProgressProvider;

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

            continueGame.OnClick += ContinueClick;
            quit.OnClick += GameLoader.Exit;
            newGame.OnClick += NewGameClick;
            settings.OnClick += settingsMenu.Open;
            authorsButton.OnClick += authorsMenu.Open;
        }

        // private void Update()
        // {
        //     //for playtests
        //     Controls controls = new Controls();
        //     if (controls.controlSchemes.KeyCode.Delete))
        //     {
        //         ResetGameData(false);
        //     }
        // }

        public void ResetGameData(bool saveLanguage = true)
        {
            var loadedLanguage = localizationTool.GetSavedLanguage();
            PlayerPrefs.DeleteAll();
            ClearAllSaveFiles();
            dataPersistenceManager.NewGame();
            if (saveLanguage) PlayerPrefs.SetString(PrefKeys.LanguageKey, loadedLanguage.ToString());
            HideContinueButton();
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
            milestoneProvider.Update();
            playerProgressProvider.Update();
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
        }

        private async void StartNewGame()
        {
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
    }
}