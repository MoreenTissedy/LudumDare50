using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Zenject;

namespace CauldronCodebase
{
    public class EndingScreen: MonoBehaviour
    {
        private const float _ENDING_SCREEN_FADE_DURATION_ = 0.2f;
        
        [SerializeField]
        private EndingsProvider endings;

        [SerializeField] private GameObject background;
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
        [SerializeField] private Transform root;

        public event Action OnClose;
        private bool active;

        public bool IsOpened => active;

        private bool final;
        private GameObject currentCartoon;

        [Inject] private SoundManager soundManager;

        [ContextMenu("Find buttons")]
        void FindButtons()
        {
            buttons = GetComponentsInChildren<EndingScreenButton>();
            map = GetComponentInChildren<SkeletonGraphic>();
        }

        private void Start()
        {
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].OnClick += OnEndingClick;
            }
            closeButton.onClick.AddListener(Close);
        }

        private async void OnEndingClick(string tag)
        {
            //cartoons don't fade
            
            Ending ending = endings.Get(tag);
            await LoadEndingCartoon(tag);
            //picture.sprite = ending.image;
            screen.SetActive(true);
            //screenFader.alpha = 0;
            //screenFader.DOFade(1, _ENDING_SCREEN_FADE_DURATION_);
            title.text = ending.title;
            description.text = ending.text;
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            await UniTask.WaitUntil(() => Input.anyKey);
            //screenFader.DOFade(0, _ENDING_SCREEN_FADE_DURATION_).OnComplete(() =>
            //{
                screen.SetActive(false);
                
            //});
        }

        private async UniTask LoadEndingCartoon(string tag)
        {
            if (currentCartoon)
            {
                Destroy(currentCartoon);
            }
            GameObject asset = await Resources.LoadAsync<GameObject>(ResourceIdents.EndingCartoons[tag]) as GameObject;
            currentCartoon = Instantiate(asset, root);
        }

        public void Open(string endingTag = "none", bool inBook = false)
        {
            if (active)
            {
                return;
            }
            final = endingTag == EndingsProvider.FINAL;
            background.SetActive(!inBook);
            if (!inBook)
            {
                soundManager.SetMusic(Music.Ending, false);
            }
            gameObject.SetActive(true);
            closeButton.gameObject.SetActive(false);
            active = true;
            soundManager.Play(Sounds.EndingPanelFold);
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
            if (tag != "none" && !endings.Unlocked(tag))
            {
                endings.Unlock(tag);
            }
            if (buttonToUnlock != null)
            {
                await UniTask.Delay(250);
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
            screen.SetActive(false);
            closeButton.gameObject.SetActive(false);
            soundManager.Play(Sounds.EndingPanelFold);
            map.AnimationState.SetAnimation(0, foldAnimation, false).Complete += (_) =>
            {
                gameObject.SetActive(false);
                OnClose?.Invoke();
            };
            for (var index = buttons.Length-1; index >=0; index--)
            {
                var button = buttons[index];
                button.Hide();
            }
        }
    }
}