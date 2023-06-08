using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Universal;

namespace CauldronCodebase
{
    public class EndingScreenButton : GrowOnMouseEnter
    {
        const float _FADE_IN_DURATION_ = 0.4f;
        const float _FADE_OUT_DURATION_ = 0.2f;
        
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject effect;
        [SerializeField] private GameObject background;
        [SerializeField] private AnimatorEventCallback animator;
        public string Tag;

        private bool active;
        public event Action<string> OnClick;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public async void Show(bool unlocked)
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 0;
            background.SetActive(unlocked);
            effect.SetActive(false);
            await canvasGroup.DOFade(1, _FADE_IN_DURATION_);
            active = unlocked;
        }

        public async void Unlock()
        {
            effect.SetActive(true);
            await UniTask.WaitUntil(() => animator.callbackReceived);
            background.SetActive(true);
            active = true;
        }

        public async void Hide()
        {
            await canvasGroup.DOFade(0, _FADE_OUT_DURATION_);
            gameObject.SetActive(false);
            effect.SetActive(false);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!active)
            {
                return;
            }
            OnClick?.Invoke(Tag);
            base.OnPointerClick(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!active)
            {
                return;
            }
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!active)
            {
                return;
            }
            base.OnPointerExit(eventData);
        }
    }
}