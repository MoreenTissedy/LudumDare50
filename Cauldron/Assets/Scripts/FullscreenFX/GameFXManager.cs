using System.Threading;
using Cysharp.Threading.Tasks;
using EasyLoc;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace CauldronCodebase
{
    public class GameFXManager : MonoBehaviour
    {
        [SerializeField] private StartGameFX startGameFX; 
        [SerializeField] private DayStageFX sunFX;
        [SerializeField] private DayStageFX moonFX;
        [SerializeField] private EndGameFX endGameFX;
        [Localize] [SerializeField] private string startDayText = "Восходит солнце";
        [Localize] [SerializeField] private string endGameText = "Наступает ночь";
        [Localize] [SerializeField] private string startGameSaveHint = "This means the cat is saving your progress. Do not switch off.";

        private SoundManager soundManager;
        private EndingsProvider endingsProvider;

        private BaseFX currentEffect;
        private CancellationTokenSource cancellationTokenSource;

        [Inject]
        private void Construct(SoundManager soundManager, EndingsProvider endingsProvider)
        {
            this.soundManager = soundManager;
            this.endingsProvider = endingsProvider;
        }

        public async UniTask ShowStartGame()
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main_desktop"));
            await PlayFX(CreateFx(startGameFX).SetSavingProcessHint(startGameSaveHint));
        }

        public async UniTask ShowSunrise()
        {
            await PlayFX(CreateFx(sunFX).SetText(startDayText));
        }

        public async UniTask ShowSunset()
        {
            await PlayFX(CreateFx(moonFX).SetText(endGameText));
        }

        public async UniTask ShowEndGame(string ending)
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
