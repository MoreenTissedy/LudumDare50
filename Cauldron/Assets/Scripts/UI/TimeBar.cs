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

        private MainSettings settings;
        private GameStateMachine gameStateMachine;
        private GameDataHandler gameDataHandler;

        private int currentDay;
        [Inject]
        private void Construct(MainSettings settings,
                               GameStateMachine gameStateMachine,
                               GameDataHandler gameDataHandler)
        {
            this.settings = settings;
            this.gameStateMachine = gameStateMachine;
            this.gameDataHandler = gameDataHandler;
        }

        private void Start()
        {
            rectWidth = fullCycleSample.rect.width;
            currentDay = gameDataHandler.currentDay + 1;
            NewDayReset(currentDay);
            
            step = rectWidth/(settings.gameplay.cardsPerDay+3);
            
            int startSteps = 2 + gameDataHandler.cardsDrawnToday;
            ShiftBarOnLoad(gameStateMachine.currentGamePhase == GameStateMachine.GamePhase.Night ? 
                            startSteps + 1 : startSteps);

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

            float shift = step;
            if (gameDataHandler.cardsDrawnToday == 0)
            {
                shift *= 2; //longer shift for first visitor
            }
            timeBar.DOLocalMoveX(timeBar.anchoredPosition.x - shift, speed).SetEase(Ease.InOutSine);
        }

        private void OnNewDay(GameStateMachine.GamePhase phase)
        {
            if (phase != GameStateMachine.GamePhase.Night) return;
            currentDay++;

            //longer shift before night
            timeBar.DOLocalMoveX(timeBar.anchoredPosition.x - step*2, speed).
                SetEase(Ease.InOutSine).
                OnComplete(() => NewDayReset(currentDay));
        }

        void NewDayReset(int day)
        {
            string dayNumberText = day.ToString();
            dayNumber.text = dayNumberText;
            timeBar.anchoredPosition = new Vector2(-rectWidth / 2, 0);
        }

        private void ShiftBarOnLoad(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                timeBar.anchoredPosition = new Vector2(timeBar.anchoredPosition.x - step, 0);
            }
        }
    }
}