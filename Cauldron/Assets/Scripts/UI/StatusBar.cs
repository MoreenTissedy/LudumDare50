using System;
using CauldronCodebase;
using CauldronCodebase.GameStates;
using DG.Tweening;
using UnityEngine;
using TMPro;
using  UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Zenject;

namespace CauldronCodebase
{
    //TODO: 3 separate scripts, bar, icon, tooltip
    public class StatusBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform mask;
        public GameObject effect;
        public RectTransform maskTemp;
        public GameObject effectTemp;
        public float gradualReduce = 3f;
        public float effectDelay = 1.5f;
        [FormerlySerializedAs("sign")] public RectTransform symbol;
        public GameObject signCritical;
        public float criticalThreshold = 0.2f;
        public float signSizeChangeTime = 0.5f;
        public float signSizeChange = 1.3f;
        public bool vertical;
        public Statustype type;
        private Text tooltip;
        
        private float initialDimension;

        private GameDataHandler gameDataHandler;
        private MainSettings settings;
        private int currentValue = Int32.MinValue;

        [Inject]
        private void Construct(MainSettings mainSettings, GameDataHandler dataHandler)
        { 
           gameDataHandler = dataHandler;
           settings = mainSettings;
           tooltip = GetComponentInChildren<Text>();
           if (tooltip != null)
           {   
               tooltip.gameObject.SetActive(false);
           }

           initialDimension = vertical ? mask.rect.height : mask.rect.width;
           gameDataHandler.StatusChanged += UpdateValue;
           
           effect.SetActive(false);
           effectTemp.SetActive(false);
           signCritical.SetActive(false);
        }

        private void Start()
        {
            SetValue(this.gameDataHandler.Get(type), false);
        }

        private void UpdateValue()
        {
            SetValue(gameDataHandler.Get(type));
        }

        private void SetValue(int current, bool animate = true)
        {
            if (current == currentValue)
            {
                return;
            }
            bool statusIncrease = currentValue < current;
            currentValue = current;
            var newSize = CalculateMaskSize(current);
            if (animate)
            {
                var animations = DOTween.Sequence();
                RectTransform firstMask = statusIncrease ? maskTemp : mask;
                GameObject theEffect = statusIncrease ? effectTemp : effect;
                RectTransform secondMask = statusIncrease ? mask : maskTemp;
                GrowSymbol();
                animations
                    .AppendCallback(() => theEffect.SetActive(true))
                    .Append(firstMask.DOSizeDelta(newSize, gradualReduce))
                    .AppendInterval(effectDelay)
                    .AppendCallback(() => theEffect.SetActive(false))
                    .Append(secondMask.DOSizeDelta(newSize, gradualReduce));
            }
            else
            {
                mask.sizeDelta = newSize;
                maskTemp.sizeDelta = newSize;
            }
        }

        private Vector2 CalculateMaskSize(int current)
        {
            float ratio = (float) current / settings.statusBars.Total;
            ratio = Mathf.Clamp(ratio, 0, 1);
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

            return newSize;
        }

        void GrowSymbol()
        {
            if (currentValue < criticalThreshold * settings.statusBars.Total 
                || currentValue > (1-criticalThreshold) * settings.statusBars.Total)
            {
                signCritical.SetActive(true);
            }
            else
            {
                signCritical.SetActive(false);
            }
            symbol.DOSizeDelta(symbol.sizeDelta * signSizeChange, signSizeChangeTime).
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


    
