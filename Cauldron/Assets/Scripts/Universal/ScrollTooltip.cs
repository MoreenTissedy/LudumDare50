using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Universal
{
    public class ScrollTooltip : MonoBehaviour
    {
        public Canvas canvas;
        public RectTransform scroll;
        public CanvasGroup scrollFader;
        public TMP_Text textField;
        public CanvasGroup textFader;
        public float startScrollWidth = 170;
        public float scrollDuration = 0.7f;
        public float scrollFadeDuration = 0.3f;
        public Ease scrollOutEase;
        public Ease scrollInEase;
        public float textFadeDelay = 0.5f;
        public float textFadeDuration = 0.3f;
        
        [Header("For tests use context menu")]
        public string testText = "Корень мандрагоры";

        private Sequence tweenSequence;
        private ContentSizeFitter fitter;
        private float targetWidth;

        private void Start()
        {
            canvas.enabled = false;
            WaitLayoutAndSave().Forget();
        }

        private async UniTaskVoid WaitLayoutAndSave()
        {
            await UniTask.DelayFrame(2);
            targetWidth = scroll.sizeDelta.x;
        }

        [ContextMenu("TestOpen")]
        public void TestOpen()
        {
            Open(testText);
        }

        public async UniTaskVoid Open(string text)
        {
            canvas.enabled = false;
            await SetText(text);
            OpenAnimation();
        }

        public void Open()
        {
            OpenAnimation();
        }

        public async UniTask SetText(string text)
        {
            if (!fitter)
            {
                fitter = scroll.GetComponent<ContentSizeFitter>();
            }
            fitter.enabled = true;
            textField.text = text;
            if (Application.isPlaying)
            {
                await UniTask.DelayFrame(2); //to rearrange the layout
                targetWidth = scroll.sizeDelta.x;
            }
        }

        private void OpenAnimation()
        {
            if (!fitter)
            {
                fitter = scroll.GetComponent<ContentSizeFitter>();
            }
            fitter.enabled = false;
            canvas.enabled = true;
            tweenSequence?.Kill();
            tweenSequence = DOTween.Sequence();
            tweenSequence
                .Append(scroll.DOSizeDelta(new Vector2(targetWidth, scroll.sizeDelta.y), scrollDuration)
                    .From(new Vector2(startScrollWidth, scroll.sizeDelta.y))
                    .SetEase(scrollOutEase))
                .SetSpeedBased()
                .Insert(0, scrollFader.DOFade(1, scrollFadeDuration).From(0))
                .Insert(textFadeDelay, textFader.DOFade(1, textFadeDuration).From(0))
                .Play();
        }

        [ContextMenu("TestClose")]
        public void Close()
        {
            tweenSequence?.Kill();
            tweenSequence = DOTween.Sequence();
            tweenSequence
                .Append(textFader.DOFade(0, textFadeDuration))
                .Insert(0, scroll.DOSizeDelta(new Vector2(startScrollWidth, scroll.sizeDelta.y), scrollDuration)
                    .SetEase(scrollInEase))
                .SetSpeedBased()
                .Insert(0, scrollFader.DOFade(0, scrollFadeDuration))
                .AppendCallback(() => canvas.enabled = false)
                .Play();
        }

        private void OnDestroy()
        {
            tweenSequence?.Kill();
        }
    }
}