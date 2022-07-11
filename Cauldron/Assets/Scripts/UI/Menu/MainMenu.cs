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

        //TODO: if there is no save - hide New Game button and replace Continue text with New
        private void Start()
        {
            continueGame.onClick.AddListener(GameLoader.UnloadMenu);
            quit.onClick.AddListener(GameLoader.Exit);
            newGame.onClick.AddListener(NewGameClick);
        }

        private void NewGameClick()
        {
            GameLoader.ReloadGame();
            Debug.Log("game loaded");
            GameLoader.UnloadMenu();
            Debug.Log("menu unloaded");
        }
    }
}