using System;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using TMPro;
using  UnityEngine.UI;

    public class StatusBar : MonoBehaviour
    {
        public RectTransform mask;
        public float gradualReduce = 0.5f;
        public Statustype type;
        
        private float initialHeight;

        private void Start()
        {
            
           initialHeight = mask.rect.height;
           switch (type)
           {
               case Statustype.Money:
                   GameManager.instance.money.changed += () => SetValue(GameManager.instance.money.Value());
                   SetValue(GameManager.instance.money.Value());
                   break;
               case Statustype.Fear:
                   GameManager.instance.fear.changed += () => SetValue(GameManager.instance.fear.Value());
                   SetValue(GameManager.instance.money.Value());
                   break;
               case Statustype.Fame:
                   GameManager.instance.fame.changed += () => SetValue(GameManager.instance.fame.Value());
                   SetValue(GameManager.instance.money.Value());
                   break;
           }
        }


        public void SetValue(int current, bool dotween = true)
        {
            //symbol glow
            
            float ratio = (float)current / Status.max;
            ratio *= initialHeight;
            if (dotween)
            {
                mask.DOSizeDelta(new Vector2(mask.rect.width, ratio), gradualReduce)
                    .SetEase(Ease.InOutCirc);
            }
            else
            {
                mask.sizeDelta = new Vector2(ratio, mask.rect.height);
            }
        }

    }



    
