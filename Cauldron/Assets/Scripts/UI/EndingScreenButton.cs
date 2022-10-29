using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class EndingScreenButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private State currentState;
        enum  State
        {
            Locked,
            Unlocked,
            Current
        }
        
        [Inject]
        private EndingsProvider provider;
        [SerializeField] private EndingScreen panel;
        [SerializeField] private int number = -1;
        [SerializeField] private Image image;

        [SerializeField] private Sprite unlocked, locked, current, lockedHighlighted;


        #if UNITY_EDITOR
        void OnValidate()
        {
            if (panel is null)
            {
                panel = GetComponentInParent<EndingScreen>();
            }

            if (number <= 0)
            {
                number = transform.GetSiblingIndex() - 1;
            }

            if (image is null)
            {
                image = GetComponentInChildren<Image>();
            }

            if (PrefabUtility.IsPartOfPrefabInstance(this))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
        }
        #endif

        void UpdateColor(int page)
        {
            if (panel.CurrentPage == number)
            {
                currentState = State.Current;
                image.sprite = unlocked;
                image.color = Color.white;
            }
            else if (provider.Unlocked(number))
            {
                currentState = State.Unlocked;
                image.sprite = unlocked;
                image.color = Color.gray;
            }
            else
            {
                currentState = State.Locked;
                image.sprite = locked;
                image.color = Color.white;
            }
        }

        private void OnEnable()
        {
            panel.OnPageUpdate += UpdateColor;
        }

        private void OnDisable()
        {
            panel.OnPageUpdate -= UpdateColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (currentState == State.Current)
                return;
            panel.OpenPage(number);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (currentState == State.Locked)
            {
                image.sprite = lockedHighlighted;
            }
            else if (currentState == State.Unlocked)
            {
                image.color = Color.Lerp(Color.gray, Color.white, 0.5f);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
            if (currentState == State.Locked)
            {
                image.sprite = locked;
            }
            else if (currentState == State.Unlocked)
            {
                image.color = Color.gray;
            }
        }
    }
}