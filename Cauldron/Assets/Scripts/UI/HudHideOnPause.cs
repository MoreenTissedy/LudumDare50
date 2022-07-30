using System;
using UnityEngine;

namespace CauldronCodebase

{
    public class HudHideOnPause: MonoBehaviour
    {
        private void Start()
        {
            if (GameLoader.IsMenuOpen())
            {
                gameObject.SetActive(false);
            }

            GameLoader.GamePaused += OnGamePaused;
        }

        private void OnGamePaused(bool pause)
        {
            gameObject.SetActive(!pause);
        }

        private void OnDestroy()
        {
            GameLoader.GamePaused -= OnGamePaused;
        }
    }
}