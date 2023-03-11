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

        private MainSettings settings;
        private GameStateMachine gameStateMachine;
        private GameDataHandler gameDataHandler;
        private NightPanel nightPanel;

        private int currentDay;
        [Inject]
        private void Construct(MainSettings settings,
                               GameStateMachine gameStateMachine,
                               GameDataHandler gameDataHandler,
                               NightPanel nightPanel)
        {
            this.settings = settings;
            this.gameStateMachine = gameStateMachine;
            this.gameDataHandler = gameDataHandler;
            this.nightPanel = nightPanel;
        }

        private void Awake()
        {
            rectWidth = fullCycleSample.rect.width;
            currentDay = gameDataHandler.currentDay + 1;
            
            NewDayReset(currentDay);
        }

        private void Start()
        {
            step = rectWidth/(settings.gameplay.cardsPerDay+3);
            
            int startSteps = 2 + gameDataHandler.cardsDrawnToday;
            ShiftBarOnLoad(startSteps);

            gameStateMachine.OnChangeState += OnNewDay;
            gameStateMachine.OnChangeState += OnNewVisitor;
            nightPanel.OnClose += NewDayShift;
        }

        private void OnDestroy()
        {
            gameStateMachine.OnChangeState -= OnNewDay;
            gameStateMachine.OnChangeState -= OnNewVisitor;
            nightPanel.OnClose -= NewDayShift;
        }

        private void OnNewVisitor(GameStateMachine.GamePhase phase)
        {
            if (phase != GameStateMachine.GamePhase.Visitor) return;

            if(gameDataHandler.cardsDrawnToday == 0) return;
            timeBar.DOLocalMoveX(timeBar.anchoredPosition.x-step, speed);
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
            
            dayNumber.text = $"{dayText} {day}";
            timeBar.anchoredPosition = new Vector2(-rectWidth / 2, 0);
        }

        private void NewDayShift()
        {
            timeBar.DOLocalMoveX(timeBar.anchoredPosition.x - step * 2, speed).SetEase(Ease.InOutSine);
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