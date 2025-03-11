using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class EndingScreenButton : MonoBehaviour
    {
        const float _FADE_IN_DURATION_ = 0.4f;
        const float _FADE_OUT_DURATION_ = 0.2f;
        
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject effect;
        [SerializeField] private GameObject background;
        [SerializeField] private AnimatorEventCallback animator;
        [SerializeField] private Sounds sound = Sounds.EndingUnlock;
        [SerializeField] private FlexibleButton button;

        public string Tag;

        public event Action<string> OnClick;

        [Inject] private SoundManager soundManager;

        private void Start()
        {
            button.OnClick += Click;
            gameObject.SetActive(false);
        }

        public async void Show(bool unlocked)
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 0;
            background.SetActive(unlocked);
            effect.SetActive(false);
            soundManager.Play(Sounds.EndingButtonAppear);
            await canvasGroup.DOFade(1, _FADE_IN_DURATION_);
            button.IsInteractive = unlocked;
        }

        public async void Unlock()
        {
            soundManager.Play(sound);
            effect.SetActive(true);
            await UniTask.WaitUntil(() => animator.callbackReceived);
            background.SetActive(true);
            button.IsInteractive = true;
        }

        public async void Hide()
        {
            await canvasGroup.DOFade(0, _FADE_OUT_DURATION_);
            gameObject.SetActive(false);
            effect.SetActive(false);
        }

        public void Click()
        {
            OnClick?.Invoke(Tag);
        }

        private void OnDestroy()
        {
            button.OnClick -= Click;
        }
    }
}