using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class FadeController : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;
        [SerializeField] [Range(0f, 1f)] private float fadeAlpha;
        [SerializeField] [Tooltip("Fade in seconds")] private float fadeDuration;

        public async UniTask FadeIn(float duration, float targetAlpha)
        {
            fadeImage.gameObject.SetActive(true);
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsedTime / duration * targetAlpha);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
                await UniTask.Yield();
            }

        }
        
        public async UniTask FadeIn()
        {
            fadeImage.gameObject.SetActive(true);
            float elapsedTime = 0;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration * fadeAlpha);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
                await UniTask.Yield();
            }
        }
        
        public async UniTask FadeOut()
        {
            float initialAlpha = fadeImage.color.a;
            float elapsedTime = 0;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(initialAlpha, 0, elapsedTime / fadeDuration);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
                await UniTask.Yield();
            }
            fadeImage.gameObject.SetActive(false);
            
        }
        
        public async UniTask FadeOut(float duration)
        {
            float initialAlpha = fadeImage.color.a;
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(initialAlpha, 0, elapsedTime / duration);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
                await UniTask.Yield();
            }
            fadeImage.gameObject.SetActive(false);
            
        }
    }
    
    
}
