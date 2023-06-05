using System.Collections;
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
        public Button settings;
        public SettingsMenu settingsMenu;
        [SerializeField] private Button authorsButton;
        [SerializeField] private GameObject authorsPanel;
        [SerializeField] private Image fadeImage;
        [SerializeField] private float fadeDuration;

        [Inject] private DataPersistenceManager dataPersistenceManager;

        private void OnValidate()
        {
            if (!settingsMenu) settingsMenu = FindObjectOfType<SettingsMenu>();
            if (!authorsButton) authorsButton = GameObject.Find("AuthorsButton").GetComponent<Button>();
            if (!authorsPanel) authorsPanel = GameObject.Find("Authors_panel");
        }
        
        private void Start()
        {
            if (!PlayerPrefs.HasKey(FileDataHandler.PrefSaveKey))
            {
                HideContinueButton();
            }
            continueGame.onClick.AddListener(ContinueClick);
            quit.onClick.AddListener(GameLoader.Exit);
            newGame.onClick.AddListener(NewGameClick);
            settings.onClick.AddListener(settingsMenu.Open);
            authorsButton.onClick.AddListener(ShowAuthors);
        }

        private void Update()
        {
            //for playtests
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                PlayerPrefs.DeleteAll();
                dataPersistenceManager.NewGame();
                HideContinueButton();
                Debug.LogWarning("All data cleared!");
            }
        }

        private void HideContinueButton()
        {
            continueGame.gameObject.SetActive(false);
        }

        private void NewGameClick()
        {
            StartCoroutine(FadeImage(fadeDuration));
        }

        private void ContinueClick()
        {
            GameLoader.UnloadMenu();
        }

        private void StartNewGame()
        {
            GameLoader.ReloadGame();
            dataPersistenceManager.NewGame();
        }

        private void ShowAuthors()
        {
            authorsPanel.SetActive(true);
        }
        
        IEnumerator FadeImage(float duration)
        {
            fadeImage.gameObject.SetActive(true);
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
                Debug.Log(alpha);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
                yield return null;
            }
            
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
        
    }
}