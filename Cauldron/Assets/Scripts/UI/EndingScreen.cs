using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Spine.Unity;

namespace CauldronCodebase
{
    public class EndingScreen: MonoBehaviour
    {
        private const float _ENDING_SCREEN_FADE_DURATION_ = 0.2f;
        
        [Inject]
        private EndingsProvider endings;

        [SerializeField] private SkeletonGraphic map;
        [SerializeField] private EndingScreenButton[] buttons;
        [SerializeField] private Button closeButton; 
        [SpineAnimation(dataField: "map")] [SerializeField] private string startAnimation;
        [SpineAnimation(dataField: "map")] [SerializeField] private string foldAnimation;
        
        [Header("Ending display")] 
        [SerializeField] private GameObject screen;
        [SerializeField] private CanvasGroup screenFader;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Image picture;

        public event Action OnClose;
        private bool active;

        private bool final;

        [ContextMenu("Find buttons")]
        void FindButtons()
        {
            buttons = GetComponentsInChildren<EndingScreenButton>();
            map = GetComponentInChildren<SkeletonGraphic>();
        }

        private void Start()
        {
            gameObject.SetActive(false);
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].OnClick += OnEndingClick;
            }
            closeButton.onClick.AddListener(Close);
        }

        private async void OnEndingClick(string tag)
        {
            Ending ending = endings.Get(tag);
            screen.SetActive(true);
            screenFader.alpha = 0;
            screenFader.DOFade(1, _ENDING_SCREEN_FADE_DURATION_);
            title.text = ending.title;
            description.text = ending.text;
            picture.sprite = ending.image;
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            await UniTask.WaitUntil(() => Input.anyKey);
            screenFader.DOFade(0, _ENDING_SCREEN_FADE_DURATION_).OnComplete(() => screen.SetActive(false));
        }

        public void Open(string endingTag = "none")
        {
            if (active)
            {
                return;
            }
            final = endingTag == EndingsProvider.FINAL;
            gameObject.SetActive(true);
            closeButton.gameObject.SetActive(false);
            active = true;
            map.AnimationState.SetAnimation(0, startAnimation, false).Complete += (_) => OnComplete(endingTag);
        }

        private async void OnComplete(string tag)
        {
            EndingScreenButton buttonToUnlock = null;
            foreach (EndingScreenButton button in buttons)
            {
                await UniTask.DelayFrame(15);
                if (button.Tag == tag)
                {
                    buttonToUnlock = button;
                }
                button.Show(endings.Unlocked(button.Tag) && button.Tag != tag);
            }
            if (!endings.Unlocked(tag))
            {
                endings.Unlock(tag);
            }
            if (buttonToUnlock != null)
            {
                await UniTask.DelayFrame(15);
                buttonToUnlock.Unlock();
                await UniTask.Delay(TimeSpan.FromSeconds(2));
                OnEndingClick(tag);
            }
            closeButton.gameObject.SetActive(true);
        }

        public async void Close()
        {
            if (!active)
            {
                return;
            }
            if (final)
            {
                Debug.LogError("Exit!");
                GameLoader.Exit();
                return;
            }
            active = false;
            closeButton.gameObject.SetActive(false);
            map.AnimationState.SetAnimation(0, foldAnimation, false).Complete += (_) =>
            {
                gameObject.SetActive(false);
                OnClose?.Invoke();
            };
            foreach (var button in buttons)
            {
                await UniTask.DelayFrame(5);
                button.Hide();
            }
        }
    }
}