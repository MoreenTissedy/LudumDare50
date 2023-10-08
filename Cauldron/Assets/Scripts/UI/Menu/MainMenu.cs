using EasyLoc;
using Save;
using UnityEngine;
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

        [Inject] private DataPersistenceManager dataPersistenceManager;
        [Inject] private FadeController fadeController;
        [Inject] private LocalizationTool locTool;
        [Inject] private SoundManager soundManager; 

        private void Start()
        {
            if (!GameLoader.IsGameLoaded())
            {
                soundManager.SetMusic(Music.Menu, false);
            }
            if (!PlayerPrefs.HasKey(FileDataHandler.PrefSaveKey))
            {
                HideContinueButton();
            }

            locTool.LoadSavedLanguage();
            continueGame.OnClick += ContinueClick;
            quit.OnClick += GameLoader.Exit;
            newGame.OnClick += NewGameClick;
            settings.OnClick += settingsMenu.Open;
            authorsButton.OnClick += authorsMenu.Open;
        }

        private void Update()
        {
            //for playtests
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                ResetGameData();
            }
        }

        public void ResetGameData()
        {
            PlayerPrefs.DeleteAll();
            dataPersistenceManager.NewGame();
            HideContinueButton();
            Debug.LogWarning("All data cleared!");
        }

        private void HideContinueButton()
        {
            continueGame.gameObject.SetActive(false);
        }

        private void NewGameClick()
        {
            switch (PlayerPrefs.HasKey(FileDataHandler.PrefSaveKey))
            {
                case true:
                    Debug.LogWarning("The saved data has been deleted and a new game has been started");
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
            GameLoader.ReloadGame();
            dataPersistenceManager.NewGame();
        }
    }
}