using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class EndingScreenButton : GrowOnMouseEnter, IPointerClickHandler
    {
        
        [Inject]
        private EndingsProvider provider;
        [SerializeField] private EndingScreen panel;
        [SerializeField] private int number = -1;
        [SerializeField] private Image image;

        [SerializeField] private Sprite unlocked, locked, current;
     
        
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
                image.sprite = unlocked;
                image.color = Color.white;
            }
            else if (provider.unlocked[number])
            {
                image.sprite = unlocked;
                image.color = Color.gray;
            }
            else
            {
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

         public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            panel.OpenPage(number);
        }
    }
}