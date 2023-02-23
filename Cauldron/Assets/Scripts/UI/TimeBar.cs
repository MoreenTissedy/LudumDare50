using CauldronCodebase.GameStates;
using DG.Tweening;
using EasyLoc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class TimeBar : MonoBehaviour
    {
        public TMP_Text dayNumber;
        public RectTransform timeBar;
        public Sprite fullCycleSample;
        private float rectWidth;
        private float step;
        public float speed = 2;
        [Localize]
        public string dayText = "День";

        private MainSettings settings;
        private GameStateMachine gameStateMachine;
        private GameData gameData;

        [Inject]
        private void Construct(MainSettings settings,
                               GameStateMachine gameStateMachine,
                               GameData gameData)
        {
            this.settings = settings;
            this.gameStateMachine = gameStateMachine;
            this.gameData = gameData;
        }

        private void Awake()
        {
            rectWidth = fullCycleSample.rect.width;
            dayNumber.text = $"{dayText} 1";
            timeBar.anchoredPosition = new Vector2(-rectWidth / 2, 0);
        }

        private void Start()
        {
            step = rectWidth/(settings.gameplay.cardsPerDay+3);

            gameStateMachine.OnChangeState += OnNewDay;
            gameStateMachine.OnChangeState += OnNewVisitor;
        }

        private void OnDestroy()
        {
            gameStateMachine.OnChangeState -= OnNewDay;
            gameStateMachine.OnChangeState -= OnNewVisitor;
        }

        private void OnNewVisitor(GameStateMachine.GamePhase phase)
        {
            if (phase != GameStateMachine.GamePhase.Visitor) return;
            float newStep = step;
            //longer shift after night
            if (gameData.cardsDrawnToday == 0)
                newStep = step * 2;
            timeBar.DOLocalMoveX(timeBar.anchoredPosition.x-newStep, speed);
        }

        private void OnNewDay(GameStateMachine.GamePhase phase)
        {
            if (phase != GameStateMachine.GamePhase.Night) return;
            //longer shift before night
            timeBar.DOLocalMoveX(timeBar.anchoredPosition.x - step*2, speed).
                SetEase(Ease.InOutSine).
                OnComplete(() => NewDayReset(gameData.currentDay + 1));
        }

        void NewDayReset(int day)
        {
            dayNumber.text = $"{dayText} {day}";
            timeBar.anchoredPosition = new Vector2(-rectWidth / 2, 0);
        }

    }
}