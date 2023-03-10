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
        public Text dayNumber;
        public RectTransform timeBar;
        public Sprite fullCycleSample;
        private float rectWidth;
        private float step;
        public float speed = 2;
        [Localize]
        public string dayText = "День";

        private MainSettings settings;
        private GameStateMachine gameStateMachine;
        private GameDataHandler gameDataHandler;

        [Inject]
        private void Construct(MainSettings settings,
                               GameStateMachine gameStateMachine,
                               GameDataHandler gameDataHandler)
        {
            this.settings = settings;
            this.gameStateMachine = gameStateMachine;
            this.gameDataHandler = gameDataHandler;
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
            if (gameDataHandler.cardsDrawnToday == 0)
                newStep = step * 2;
            timeBar.DOLocalMoveX(timeBar.anchoredPosition.x-newStep, speed);
        }

        private void OnNewDay(GameStateMachine.GamePhase phase)
        {
            if (phase != GameStateMachine.GamePhase.Night) return;
            //int day = gameDataHandler.currentDay;
            //longer shift before night
            if (gameDataHandler == null || gameDataHandler.currentDay == null)
            {
                Debug.Log("Troubles with game data handler");
                return;
            }
            timeBar.DOLocalMoveX(timeBar.anchoredPosition.x - step*2, speed).
                SetEase(Ease.InOutSine).
                OnComplete(() => NewDayReset(gameDataHandler.currentDay));
        }

        void NewDayReset(int day)
        {
            
            dayNumber.text = $"{dayText} {day}";
            timeBar.anchoredPosition = new Vector2(-rectWidth / 2, 0);
        }

    }
}