using System;
using DG.Tweening;
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
        [SerializeField] private RectTransform trans;
        
        private GameDataHandler gameDataHandler;
        public event Action OnClick;

        private bool flashedOnce;
        
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
                
                //start flashing to attract attention
                if (gameDataHandler.IsEnoughMoneyForRumours() && !flashedOnce)
                {
                    trans.DOSizeDelta(Vector3.one*sizeCoef, sizeSpeed)
                        .SetLoops(-1, LoopType.Yoyo);
                    flashedOnce = true;
                }
                else
                {
                    trans.localScale = Vector3.one;
                }
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            OnClick?.Invoke();
            trans.DOKill(true);
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