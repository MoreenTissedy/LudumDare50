using System;
using CauldronCodebase.GameStates;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    //TODO: separate scripts: bar, icon
    public class StatusBar : MonoBehaviour
    {
        public RectTransform mask;
        public float maskTailPercent = 6f;
        public bool keepTailEmpty;
        public ParticleFadeController effect;
        public RectTransform maskTemp;
        public ParticleFadeController effectTemp;
        public float gradualReduce = 3f;
        public float effectDelay = 1.5f;
        public RectTransform symbol;
        public GameObject signCritical;
        public float criticalThreshold = 0.2f;
        public float signSizeChangeTime = 0.5f;
        public float signSizeChange = 1.3f;
        public bool vertical;
        public Statustype type;
        
        private float initialDimension;

        private GameDataHandler gameDataHandler;
        private MainSettings settings;

        private int currentValue = Int32.MinValue;

        private float gameMaskLength;

        [Inject]
        private void Construct(MainSettings mainSettings, GameDataHandler dataHandler, GameStateMachine gameStateMachine)
        { 
           gameDataHandler = dataHandler;
           settings = mainSettings;
           gameStateMachine.OnGameStarted += StartGame;

           initialDimension = vertical ? mask.rect.height : mask.rect.width;
           gameMaskLength = initialDimension - initialDimension * maskTailPercent / 100;
           signCritical.SetActive(false);
        }

        private void StartGame()
        {
            SetValue(this.gameDataHandler.Get(type), false);
            gameDataHandler.StatusChanged += UpdateValue;
        }

        private void UpdateValue(Statustype statusType, int i)
        {
            if (statusType != type || i == 0)
            {
                return;
            }
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
                if (DOTween.IsTweening(this))
                {
                    DOTween.Kill(this);
                    effectTemp.Hide();
                    effect.Hide();
                }
                
                var animations = DOTween.Sequence(this);
                RectTransform firstMask = statusIncrease ? maskTemp : mask;
                var theEffect = statusIncrease ? effectTemp : effect;
                RectTransform secondMask = statusIncrease ? mask : maskTemp;
                GrowSymbol();
                animations
                    .AppendCallback(() =>
                    {
                        if(current < settings.statusBars.Total)
                            theEffect.Show().Forget();
                        else
                        {
                            theEffect.Blink().Forget();
                        }
                    })
                    .Append(firstMask.DOSizeDelta(newSize, gradualReduce))
                    .AppendInterval(effectDelay)
                    .AppendCallback(() => theEffect.Hide())
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
            
            if (keepTailEmpty && ratio < 1)
            {
                ratio *= gameMaskLength;
            }
            else
            {
                ratio *= initialDimension;
            }
            
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

    }
}


    
