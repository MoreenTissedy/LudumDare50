using System;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using TMPro;
using  UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class StatusBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform mask;
        public RectTransform sign;
        public float gradualReduce = 3f;
        public float signSizeChangeTime = 0.5f;
        public float signSizeChange = 1.3f;
        public bool vertical;
        public Statustype type;
        private Text tooltip;
        
        private float initialDimension;

        private void Start()
        {
           tooltip = GetComponentInChildren<Text>();
           if (tooltip != null)
            {   
                tooltip.gameObject.SetActive(false);
            }


           if (vertical)
           {
               initialDimension = mask.rect.height;
           }
           else
           {
               initialDimension = mask.rect.width;
           }
           
           switch (type)
           {
               case Statustype.Money:
                   GameManager.instance.money.changed += () => SetValue(GameManager.instance.money.Value());
                   SetValue(GameManager.instance.money.Value());
                   break;
               case Statustype.Fear:
                   GameManager.instance.fear.changed += () => SetValue(GameManager.instance.fear.Value());
                   SetValue(GameManager.instance.fear.Value());
                   break;
               case Statustype.Fame:
                   GameManager.instance.fame.changed += () => SetValue(GameManager.instance.fame.Value());
                   SetValue(GameManager.instance.fame.Value());
                   break;
           }
        }


        public void SetValue(int current, bool dotween = true)
        {
            //symbol glow
            
            float ratio = (float)current / Status.max;
            ratio *= initialDimension;
            Vector2 newSize;
            if (vertical)
            {
                newSize = new Vector2(mask.rect.width, ratio);
            }
            else
            {
                newSize = new Vector2(ratio, mask.rect.height);
            }
            if (dotween)
            {
                
                    mask.DOSizeDelta(newSize, gradualReduce)
                        .SetEase(Ease.InOutCirc);
            }
            else
            {
                mask.sizeDelta = newSize;
            }
            
            //grow symbol
            GrowSymbol();
        }

        void GrowSymbol()
        {
            sign.DOSizeDelta(sign.sizeDelta * signSizeChange, signSizeChangeTime).
                SetLoops(2, LoopType.Yoyo).
                SetEase(Ease.InQuad);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltip is null)
                return;
            tooltip.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltip is null)
                return;
            tooltip.gameObject.SetActive(false);
        }

    }
}


    
