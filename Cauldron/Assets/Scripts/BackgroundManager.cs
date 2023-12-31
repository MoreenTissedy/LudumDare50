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
            soundManager.SetMusic(locationIndex == 0 ? Music.Location1 : Music.Location2, true);
        }
    }
}