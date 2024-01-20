using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class FadeController : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;
       
        public async UniTask FadeIn(float startAlpha = 0f, float endAlpha = 1f, float duration = 0.3f)
        {
            await Fade(startAlpha, endAlpha,  duration);
        }

        public async UniTask FadeOut(float endAlpha = 0f, float duration = 0.3f)
        {
            await Fade(fadeImage.color.a, endAlpha, duration);
        }
        
        private async UniTask Fade(float startAlpha, float endAlpha, float duration)
        {
            fadeImage.gameObject.SetActive(true);
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
                await UniTask.Yield();
            }

            if (endAlpha == 0)
            {
                fadeImage.gameObject.SetActive(false);
            }
        }
    }
    
    
}
