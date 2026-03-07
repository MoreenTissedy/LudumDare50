using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public enum FadeMode
    {
        Preserve,
        UnderPopup,
        OverPopup
    } 
    public class FadeController : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private int underPopupSorting = 100;
        [SerializeField] private int overPopupSorting = 1000;
        [SerializeField] private Image fadeImage;
        [SerializeField] private GameObject waitNotification;

        private CancellationTokenSource cts;

        public async UniTask FadeIn(float startAlpha = 0f, float endAlpha = 1f, float duration = 0.3f, FadeMode mode = FadeMode.UnderPopup, OverlayManager blockInput = null, bool showWait = false)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            if (blockInput != null)
            {
                blockInput.LockCurrentLayer(true);
            }
            Action onDone = default;
            if (showWait)
            {
                onDone = () => waitNotification.SetActive(true);
            }
            await Fade(startAlpha, endAlpha,  duration, cts.Token, mode, onDone);
        }

        public async UniTask FadeOut(float endAlpha = 0f, float duration = 0.3f, FadeMode mode = FadeMode.Preserve, OverlayManager unblockInput = null)
        {
            waitNotification.SetActive(false);
            cts?.Cancel();
            cts = new CancellationTokenSource();

            Action onDone = default;
            if (unblockInput != null)
            {
                onDone = () => unblockInput.LockCurrentLayer(false);
            }
            await Fade(fadeImage.color.a, endAlpha, duration, cts.Token, mode, onDone);
        }
        
        private async UniTask Fade(float startAlpha, float endAlpha, float duration, CancellationToken cancellationToken, FadeMode mode, Action onDone = default)
        {
            switch (mode)
            {
                case FadeMode.UnderPopup:
                    canvas.sortingOrder = underPopupSorting;
                    break;
                case FadeMode.OverPopup:
                    canvas.sortingOrder = overPopupSorting;
                    break;
            }
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
            
            onDone?.Invoke();

            if (endAlpha == 0)
            {
                fadeImage.gameObject.SetActive(false);
            }
        }
    }
    
    
}
