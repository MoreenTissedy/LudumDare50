using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CauldronCodebase
{
    //additive scene loader
    public static class GameLoader
    {
        public static event Action<bool> GamePaused;

        public static void ReloadGame()
        {
            if (IsGameLoaded())
            {
                var operation = SceneManager.UnloadSceneAsync(1);
                operation.completed += OnUnloadComplete;
            }
            else
            {
                SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            }
        }

        private static void OnUnloadComplete(AsyncOperation unloadOperation)
        {
            var operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            operation.completed += (_) =>
            {
                UnloadMenu();
            };
        }

        public static bool IsMenuOpen()
        {
            return SceneManager.GetSceneByBuildIndex(0).isLoaded;
        }

        public static bool IsGameLoaded()
        {
            return SceneManager.GetSceneByBuildIndex(1).isLoaded;
        }

        public static void LoadGameInBackground()
        {
            if (IsGameLoaded()) return;
            
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        }

        public static void LoadMenu()
        {
            if (SceneManager.GetSceneByBuildIndex(0).isLoaded)
                return;
            Time.timeScale = 0;
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
            GamePaused?.Invoke(true);
        }

        public static void UnloadMenu()
        {
            if (!SceneManager.GetSceneByBuildIndex(0).isLoaded)
                return;
            
            Time.timeScale = 1;
            SceneManager.UnloadSceneAsync(0);
            GamePaused?.Invoke(false);
        }

        public static void Exit()
        {
            Application.Quit();
        }
    }
}