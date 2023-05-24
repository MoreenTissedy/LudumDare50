using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace CauldronCodebase
{
    public class GameFXManager : MonoBehaviour
    {
        [SerializeField] private GameObject startGameFX, endGameFX;
        [SerializeField] private DayStageFX sunFX, moonFX;
        private SoundManager soundManager;
        private EndingsProvider endingsProvider;

        private bool effectPlaying;
        private GameObject currentEffect;

        [Inject]
        private void Construct(SoundManager soundManager, EndingsProvider endingsProvider)
        {
            this.soundManager = soundManager;
            this.endingsProvider = endingsProvider;
        }

        public async UniTask ShowStartGameFXUniTask()
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main_desktop"));

            var start = Instantiate(startGameFX);
            StartGameFX gameFX = start.GetComponentInChildren<StartGameFX>();
            gameFX.Init(soundManager);
            //gameFX.PlaySound();
            gameFX.OnEnd += () => effectPlaying = false;
            effectPlaying = true;
            await UniTask.WaitUntil(() => !effectPlaying).AsTask();
        }

        public async UniTask ShowEndGameFX(EndingsProvider.Unlocks ending)
        {
            var end = Instantiate(endGameFX);
            EndGameFX endFx = end.GetComponentInChildren<EndGameFX>();
            endFx.Init(soundManager);
            endFx.SelectEnding(endingsProvider.Get(ending));
            await endFx.Play();
        }

        public async UniTask ShowSunrise()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.7));
            var effectScript = Instantiate(sunFX);
            currentEffect = effectScript.gameObject;
            await effectScript.Play();
        }

        public async UniTask ShowSunset()
        {
            var effectScript = Instantiate(moonFX);
            currentEffect = effectScript.gameObject;
            await effectScript.Play();
        }
    }
}
