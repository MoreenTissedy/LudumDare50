using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Spine.Unity;

namespace CauldronCodebase
{
    public class EndingScreen: MonoBehaviour
    {
        [Inject]
        private EndingsProvider endings;

        [SerializeField] private SkeletonGraphic map;
        [SerializeField] private EndingScreenButton[] buttons;
        [SerializeField] private Button closeButton; 
        [SpineAnimation(dataField: "map")] [SerializeField] private string startAnimation;
        [SpineAnimation(dataField: "map")] [SerializeField] private string foldAnimation;
        
        [Header("Ending display")] 
        [SerializeField] private GameObject screen;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Image picture;

        public event Action OnClose;

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
            title.text = ending.title;
            description.text = ending.text;
            picture.sprite = ending.image;
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            await UniTask.WaitUntil(() => Input.anyKey);
            screen.SetActive(false);
        }

        public void Open(string endingTag = "none")
        {
            var unlock = !endings.Unlocked(endingTag);
            map.AnimationState.SetAnimation(0, startAnimation, false).Complete += (_) => OnComplete(endingTag);
            if (unlock)
            {
                endings.Unlock(endingTag);
            }
        }

        private void OnComplete(string tag)
        {
            foreach (var button in buttons)
            {
                button.Show(endings.Unlocked(tag), button.Tag == tag);
            }
        }

        public void Close()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            foreach (var button in buttons)
            {
                button.Hide();
            }
            map.AnimationState.SetAnimation(0, foldAnimation, false).Complete += (_) =>
            {
                gameObject.SetActive(false);
                OnClose?.Invoke();
            };
        }
    }
}