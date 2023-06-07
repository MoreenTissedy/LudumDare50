using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Universal;

namespace CauldronCodebase
{
    public class EndingScreenButton : GrowOnMouseEnter 
    {
        [SerializeField] private GameObject effect;
        [SerializeField] private Image image;
        [SerializeField] private GameObject background;
        public string Tag;

        private bool active;
        public event Action<string> OnClick;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public async void Show(bool unlocked, bool withEffect = false)
        {
            gameObject.SetActive(true);
            effect.SetActive(withEffect);
            if (withEffect)
            {
                background.SetActive(false);
                await UniTask.DelayFrame(Tag == "high money" ? 70 : 15);
            }
            background.SetActive(unlocked);
            image.color = unlocked ? Color.white : new Color (1f, 1f, 1f, 0.5f);
            active = unlocked;
        }

        public void Hide()
        {
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