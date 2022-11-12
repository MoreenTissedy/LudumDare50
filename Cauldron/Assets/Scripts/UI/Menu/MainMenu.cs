using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class MainMenu : MonoBehaviour
    {
        public Button continueGame;
        public Button quit;
        public Button newGame;
        public Button settings;
        public SettingsMenu settingsMenu;

        private void OnValidate()
        {
            if (!settingsMenu)
                settingsMenu = FindObjectOfType<SettingsMenu>();
        }

        //TODO: if there is no save - hide New Game button and replace Continue text with New
        private void Start()
        {
            continueGame.onClick.AddListener(GameLoader.UnloadMenu);
            quit.onClick.AddListener(GameLoader.Exit);
            newGame.onClick.AddListener(NewGameClick);
            settings.onClick.AddListener(settingsMenu.Open);
        }

        private void Update()
        {
            //for playtests
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                PlayerPrefs.DeleteAll();
                Debug.LogWarning("Prefs cleared!");
            }
        }

        private void NewGameClick()
        {
            GameLoader.ReloadGame();
        }
    }
}