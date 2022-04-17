using DG.Tweening;
using EasyLoc;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class TimeBar : MonoBehaviour
    {
        public Text dayNumber;
        public RectTransform timeBar;
        public Sprite fullCycleSample;
        private float rectWidth;
        private float step;
        public float speed = 2;
        [Localize]
        public string dayText = "День";

        private void Awake()
        {
            rectWidth = fullCycleSample.rect.width;
            dayNumber.text = $"{dayText} 1";
            timeBar.anchoredPosition = new Vector2(-rectWidth / 2, 0);
        }

        private void Start()
        {
            step = rectWidth/(GameManager.instance.cardsPerDay+3);
            GameManager.instance.NewDay += OnNewDay;
            GameManager.instance.NewEncounter += OnNewVisitor;
        }

        private void OnNewVisitor(int arg1, int arg2)
        {
            float newStep = step;
            //longer shift after night
            if (arg1 == 1)
                newStep = step * 2;
            timeBar.DOLocalMoveX(timeBar.anchoredPosition.x-newStep, speed);
        }

        private void OnNewDay(int obj)
        {
            //longer shift before night
            timeBar.DOLocalMoveX(timeBar.anchoredPosition.x - step*2, speed).
                SetEase(Ease.InOutSine).
                OnComplete(() => NewDayReset(obj));
        }

        void NewDayReset(int day)
        {
            dayNumber.text = $"{dayText} {day}";
            timeBar.anchoredPosition = new Vector2(-rectWidth / 2, 0);
        }

    }
}