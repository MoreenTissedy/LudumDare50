using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace CauldronCodebase
{
    public class GameFXManager : MonoBehaviour
    {
        [SerializeField] private BaseFX startGameFX, sunFX, moonFX;
        [SerializeField] private EndGameFX endGameFX;
        
        private SoundManager soundManager;
        private EndingsProvider endingsProvider;

        private BaseFX currentEffect;
        private CancellationTokenSource cancellationTokenSource;

        public GameFXManager(BaseFX startGameFX, BaseFX sunFX, BaseFX moonFX, EndGameFX endGameFX, SoundManager soundManager, EndingsProvider endingsProvider, BaseFX currentEffect, CancellationTokenSource cancellationTokenSource)
        {
            this.startGameFX = startGameFX;
            this.sunFX = sunFX;
            this.moonFX = moonFX;
            this.endGameFX = endGameFX;
            this.soundManager = soundManager;
            this.endingsProvider = endingsProvider;
            this.currentEffect = currentEffect;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        [Inject]
        private void Construct(SoundManager soundManager, EndingsProvider endingsProvider)
        {
            this.soundManager = soundManager;
            this.endingsProvider = endingsProvider;
        }

        public async UniTask ShowStartGame()
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main_desktop"));
            await PlayFX(CreateFx(startGameFX));
        }

        public async UniTask ShowSunrise()
        {
            await PlayFX(CreateFx(sunFX));
        }

        public async UniTask ShowSunset()
        {
            await PlayFX(CreateFx(moonFX));
        }

        public async UniTask ShowEndGame(EndingsProvider.Unlocks ending)
        {
            await PlayFX(CreateFx(endGameFX).SelectEnding(endingsProvider.Get(ending)));
        }

        private T CreateFx<T>(T asset) where T : BaseFX
        {
            Clear();
            T fx = Instantiate(asset);
            currentEffect = fx;
            return fx;
        }

        private async UniTask PlayFX(BaseFX fx)
        {
            fx.Init(soundManager);
            cancellationTokenSource = new CancellationTokenSource();
            await fx.Play().AttachExternalCancellation(cancellationTokenSource.Token);
            Clear();
        }

        public void Clear()
        {
            if (currentEffect is null)
            {
                return;
            }
            cancellationTokenSource?.Cancel();
            Destroy(currentEffect.gameObject);
            currentEffect = null;
        }
    }
}
