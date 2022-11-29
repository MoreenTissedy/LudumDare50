using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Zenject;

namespace CauldronCodebase.GameStates
{
    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    public class StateMachine : MonoBehaviour
    {
        [SerializeField] private NightPanel _nightPanel;
        [SerializeField] private EndingScreen _endingScreen;
        
        [SerializeField] private EncounterDeckBase _cardDeck;
        private MainSettings _gameSettings;
        private NightEventProvider _nightEvents;
        private GameData _gameData;
        public NightEventProvider NightEvents => _nightEvents;

        private BaseGameState _currentGameState;

        private VisitorWaitingState _visitorWaitingState;
        private VisitorState _visitorState;
        private NightState _nightState;
        private EndGameState _endGameState;

        public VisitorWaitingState VisitorWaitingState => _visitorWaitingState;
        public VisitorState VisitorState => _visitorState;
        public NightState NightState => _nightState;
        public EndGameState EndGameState => _endGameState;

        [Inject]
        private void Construct(EncounterDeckBase deck,
                               MainSettings settings,
                               GameData gameData,
                               VisitorManager visitorManager,
                               Cauldron cauldron,
                               NightEventProvider nightEvents)
        {
            _gameSettings = settings;
            _nightEvents = nightEvents;
            _gameData = gameData;
            
            _visitorWaitingState = new VisitorWaitingState(settings, _gameData, this);
            _visitorState = new VisitorState(deck, settings, _gameData, visitorManager, cauldron, this);
            _nightState = new NightState(_gameData, _gameSettings, _nightEvents, _cardDeck, _nightPanel, this);
            _endGameState = new EndGameState(_endingScreen);
            
            _currentGameState = _visitorWaitingState;
            Debug.Log("Run StateMachine Construct method");
        }

        private void Start()
        {
            Debug.Log("StateMachine Start");
            _cardDeck.Init(_gameData);
            CatTutorial catTutorial = GetComponent<CatTutorial>();
            if (catTutorial is null)
            {
                _currentGameState.Enter();
            }
            else 
            {
                catTutorial.Start();
                catTutorial.OnEnd += StartGame;
            }
        }

        private void StartGame()
        {
            CatTutorial catTutorial = GetComponent<CatTutorial>();
            if (!(catTutorial is null))
            {
                catTutorial.OnEnd -= StartGame;
            }

            _currentGameState.Enter();
        }

        public void SwitchState(BaseGameState state, bool wait)
        {
            _currentGameState = state;

            if (wait)
            {
                StartCoroutine(WaitBeforeSwitch());
                return;
            }

            _currentGameState.Enter();
        }

        public void EndGame(EndingsProvider.Unlocks ending)
        {
            _endGameState.SetEnding(ending);
            SwitchState(_endGameState, false);
        }
        
        private IEnumerator WaitBeforeSwitch()
        {
            yield return new WaitForSeconds(_gameSettings.gameplay.villagerDelay);
            SwitchState(_currentGameState,false);
        }
    }
}