using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class CovenPopupButton: MonoBehaviour
    {
        [SerializeField] private float sizeCoef = 1.2f;
        [SerializeField] private GameObject activeGlow;
        [SerializeField] private RectTransform trans;
        
        private GameDataHandler gameDataHandler;

        private bool flashedOnce;
        
        [Inject]
        private void Construct(GameDataHandler dataHandler)
        {
            gameDataHandler = dataHandler;
        }

        public void Show()
        {
            if (gameDataHandler)
            {
                activeGlow.SetActive(gameDataHandler.IsEnoughMoneyForRumours());
                
                //start flashing to attract attention
                if (gameDataHandler.IsEnoughMoneyForRumours() && !flashedOnce)
                {
                    trans.DOScale(Vector3.one * sizeCoef, 2)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                    flashedOnce = true;
                }
                else
                {
                    trans.localScale = Vector3.one;
                }
            }
        }
    }
}