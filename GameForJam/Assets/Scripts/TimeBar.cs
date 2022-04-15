using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class TimeBar : MonoBehaviour
    {
        public Text dayNumber;
        public RectTransform timeBar;
        public RectTransform mask;
        public RectTransform timeBarLeft, timeBarRight;
        private float rectWidth;
        private float step;
        public float speed = 2;

        private void Awake()
        {
            rectWidth = mask.rect.width;
            dayNumber.text = 1.ToString();
            timeBar.anchoredPosition = new Vector2(-rectWidth / 2, 0);
        }

        private void Start()
        {
            step = rectWidth/(GameManager.instance.cardsPerDay+1);
            GameManager.instance.NewDay += OnNewDay;
            GameManager.instance.NewEncounter += OnNewVisitor;
        }

        private void OnNewVisitor(int arg1, int arg2)
        {
            timeBar.DOLocalMoveX(timeBar.anchoredPosition.x-step, speed);
        }

        private void OnNewDay(int obj)
        {
            timeBar.DOLocalMoveX(timeBar.anchoredPosition.x - step, speed).
                SetEase(Ease.InOutSine).
                OnComplete(() => NewDayReset(obj));
        }

        void NewDayReset(int day)
        {
            dayNumber.text = day.ToString();
            timeBar.anchoredPosition = new Vector2(-rectWidth / 2, 0);
        }

    }
}