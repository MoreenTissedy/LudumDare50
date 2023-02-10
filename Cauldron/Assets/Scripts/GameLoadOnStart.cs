using System;
using System.Collections;
using UnityEngine;

namespace CauldronCodebase
{
    public class GameLoadOnStart : MonoBehaviour
    {
        private void Start()
        {
            if (!GameLoader.IsGameLoaded())
            {
                StartCoroutine(LoadNextFrame());
            }
        }

        IEnumerator LoadNextFrame()
        {
            yield return null;
            Time.timeScale = 0;
            GameLoader.LoadGameInBackground();
        }
    }
}