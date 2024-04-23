using System;
using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class BackgroundManager: MonoBehaviour
    {
        [Inject] private GameDataHandler gameData;
        [Inject] private SoundManager soundManager;
        [Inject] private GameStateMachine gameLoop;

        private int locationIndex;

        private void Start()
        {
            if (!GameLoader.IsMenuOpen())
            {
                soundManager.StopMusic();
            }
            locationIndex = gameData.currentRound % ResourceIdents.Backgrounds.Length;
            Instantiate(Resources.Load<GameObject>(ResourceIdents.Backgrounds[locationIndex]), transform);
            gameLoop.OnGameStarted += StartLocationMusic;
        }

        private void StartLocationMusic()
        {
            string music = $"Location{locationIndex + 1}";
            if (!Enum.TryParse(music, out Music locationMusic))
            {
                locationMusic = Music.Location1;
            }
            soundManager.SetMusic(locationMusic, true);
        }
    }
}