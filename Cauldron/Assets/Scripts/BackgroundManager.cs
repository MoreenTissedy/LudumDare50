using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class BackgroundManager : MonoBehaviour
    {
        public GameObject[] backgrounds;

        [Inject] private EndingsProvider endings;
        [Inject] private SoundManager soundManager;
        [Inject] private GameStateMachine gameLoop;

        private int locationIndex;

        private void Start()
        {
            if (!GameLoader.IsMenuOpen())
            {
                soundManager.StopMusic();
            }
            locationIndex = endings.GetUnlockedEndingsCount() % backgrounds.Length;
            Instantiate(backgrounds[locationIndex], transform);
            gameLoop.OnGameStarted += StartLocationMusic;
        }

        private void StartLocationMusic()
        {
            Music music = (Music)(locationIndex + 1);
            soundManager.SetMusic(music, true);
        }
    }
}