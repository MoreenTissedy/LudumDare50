using System.IO;
using EasyLoc;
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
        [Inject] private SoundManager soundManager;
        [Inject] private LocalizationTool localizationTool;        
        [Inject] private MilestoneProvider milestoneProvider;

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

        private static void ClearAllSaveFiles()
        {
            string dataDirPath = Application.persistentDataPath;
            string SubFolder = "Saves";
            string subDirPath = Path.Combine(dataDirPath, SubFolder);
            DirectoryInfo di = new DirectoryInfo(subDirPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
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
            GameLoader.ReloadGame();
            dataPersistenceManager.NewGame();
        }
    }
}