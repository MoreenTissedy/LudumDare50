using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class FadeController : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;

        private CancellationTokenSource cts; 
       
        public async UniTask FadeIn(float startAlpha = 0f, float endAlpha = 1f, float duration = 0.3f)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            await Fade(startAlpha, endAlpha,  duration, cts.Token);
        }

        public async UniTask FadeOut(float endAlpha = 0f, float duration = 0.3f)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            await Fade(fadeImage.color.a, endAlpha, duration, cts.Token);
        }
        
        private async UniTask Fade(float startAlpha, float endAlpha, float duration, CancellationToken cancellationToken)
        {
            fadeImage.gameObject.SetActive(true);
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                elapsedTime += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
                await UniTask.Yield(cancellationToken);
            }

            if (endAlpha == 0)
            {
                fadeImage.gameObject.SetActive(false);
            }
        }
    }
    
    
}
