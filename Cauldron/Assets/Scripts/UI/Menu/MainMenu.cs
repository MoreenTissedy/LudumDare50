using EasyLoc;
using Save;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class MainMenu : MonoBehaviour
    {
        public Button continueGame;
        public Button quit;
        public Button newGame;
        public Button VKRedirect;
        public Button DiscordRedirect;

        [Header("Settings")] public Button settings;
        public SettingsMenu settingsMenu;

        [Header("Authors")] [SerializeField] private AuthorsMenu authorsMenu;
        [SerializeField] private Button authorsButton;

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
                soundManager.SetMusic(Sounds.Menu, false);
            }
            if (!PlayerPrefs.HasKey(FileDataHandler.PrefSaveKey))
            {
                HideContinueButton();
            }

            locTool.LoadSavedLanguage();
            continueGame.onClick.AddListener(ContinueClick);
            quit.onClick.AddListener(GameLoader.Exit);
            newGame.onClick.AddListener(NewGameClick);
            settings.onClick.AddListener(settingsMenu.Open);
            authorsButton.onClick.AddListener(authorsMenu.Open);
            VKRedirect.onClick.AddListener(() => Application.OpenURL("https://vk.com/theironhearthg"));
            DiscordRedirect.onClick.AddListener(() => Application.OpenURL("https://discord.gg/pUfAGsYDDw"));
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