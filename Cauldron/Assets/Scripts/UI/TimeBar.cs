using CauldronCodebase.GameStates;
using DG.Tweening;
using EasyLoc;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
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

        [Inject]
        private GameManager gm;

        [Inject] private StateMachine _stateMachine;

        [Inject] private MainSettings settings;

        private void Awake()
        {
            rectWidth = fullCycleSample.rect.width;
            dayNumber.text = $"{dayText} 1";
            timeBar.anchoredPosition = new Vector2(-rectWidth / 2, 0);
        }

        private void Start()
        {
            step = rectWidth/(settings.gameplay.cardsPerDay+3);
            _stateMachine.NightState.NewDay += OnNewDay;
            _stateMachine.VisitorState.NewEncounter += OnNewVisitor;
        }

        private void OnDestroy()
        {
            _stateMachine.NightState.NewDay -= OnNewDay;
            _stateMachine.VisitorState.NewEncounter -= OnNewVisitor;
        }

        private void OnNewVisitor(int arg1, int arg2)
        {
            float newStep = step;
            //longer shift after night
            if (arg1 == 0)
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