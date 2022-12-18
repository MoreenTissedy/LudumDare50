using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class SimpleVisitorTimer : VisitorTimer
    {
        public Image timer;
        public float timerChangeSpeed = 1f;
        
        
        private float realTimerValue;
        private float attemptsTimerStep;
        
        public override void ReduceTimer()
        {
            realTimerValue = Mathf.Clamp(realTimerValue-attemptsTimerStep, 0, 1);
            var newFillAmount = Mathf.Lerp(0.5f, 1f, realTimerValue);
            timer.DOFillAmount(newFillAmount, timerChangeSpeed);
        }

        public override void ResetTimer(int attempts)
        {
            timer.fillAmount = 1f;
            realTimerValue = 1f;
            attemptsTimerStep = 1f / attempts;
        }
    }
}