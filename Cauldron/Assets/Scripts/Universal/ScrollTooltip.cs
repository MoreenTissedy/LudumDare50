using CauldronCodebase;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

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
        [SerializeField] private GraphicRaycaster raycaster;
        
        [Header("For tests use context menu")]
        public string testText = "Корень мандрагоры";

        private Sequence tweenSequence;
        private ContentSizeFitter fitter;
        private float targetWidth;

        [Inject] private CatAnimations catAnimations;
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
            Open(testText).Forget();
        }

        public async UniTask Open(string text)
        {
            if(catAnimations.IsDragged) return;
            
            canvas.enabled = false;
            await SetText(text);
            OpenAnimation();
        }

        public void Open()
        {
            if(catAnimations.IsDragged) return;
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
            
            if (raycaster != null)
            {
                raycaster.enabled = true;
            }
        }

        [ContextMenu("TestClose")]
        public void Close()
        {
            tweenSequence?.Kill();
            if (!canvas.enabled)
            {
                return;
            }

            tweenSequence = DOTween.Sequence();
            float delay = 0;
            if (textFader.alpha > 0)
            {
                delay = textFadeDuration;
                tweenSequence
                    .Append(textFader.DOFade(0, textFadeDuration));
            }
            tweenSequence.Insert(delay,
                    scroll.DOSizeDelta(new Vector2(startScrollWidth, scroll.sizeDelta.y), scrollDuration)
                        .SetEase(scrollInEase))
                .SetSpeedBased()
                .Insert(delay, scrollFader.DOFade(0, scrollFadeDuration))
                .AppendCallback(() => canvas.enabled = false)
                .Play();
            
            if (raycaster != null)
            {
                raycaster.enabled = false;
            }
        }

        private void OnDestroy()
        {
            tweenSequence?.Kill();
        }
        
        public async UniTask<bool> ShowAsDialog(string text, Button acceptButton, Button rejectButton)
        {
            bool accepted = false;
            bool rejected = false;
            acceptButton.onClick.AddListener(() => accepted = true);
            rejectButton.onClick.AddListener(() => rejected = true);
            acceptButton.gameObject.SetActive(true);
            rejectButton.gameObject.SetActive(true);
            
            await Open(text);
            await UniTask.WaitUntil(() => accepted || rejected);
            
            acceptButton.onClick.RemoveAllListeners();
            rejectButton.onClick.RemoveAllListeners();
            Close();
            acceptButton.gameObject.SetActive(false);
            rejectButton.gameObject.SetActive(false);
            return accepted;
        }
    }
}