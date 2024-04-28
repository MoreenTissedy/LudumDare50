using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class CovenPopupButton: GrowOnMouseEnter
    {
        [SerializeField] private ScrollTooltip scrollTooltip;
        [SerializeField] private GameObject activeGlow;
        
        private GameDataHandler gameDataHandler;
        public event Action OnClick;
        
        [Inject]
        private void Construct(GameDataHandler dataHandler)
        {
            gameDataHandler = dataHandler;
        }

        private void OnEnable()
        {
            if (gameDataHandler)
            {
                activeGlow.SetActive(gameDataHandler.IsEnoughMoneyForRumours());
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            OnClick?.Invoke();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            scrollTooltip?.Open();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            scrollTooltip?.Close();
        }
    }
}