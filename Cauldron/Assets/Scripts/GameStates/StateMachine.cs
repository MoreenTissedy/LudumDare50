using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Zenject;

namespace CauldronCodebase.GameStates
{
    //Why?
    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    //Let's rename it to GameStateMachine or smth
    public class StateMachine : MonoBehaviour
    {
        //We can inject those through Zenject as well
        [SerializeField] private NightPanel _nightPanel;
        [SerializeField] private EndingScreen _endingScreen;
        [SerializeField] private EncounterDeckBase _cardDeck;
        
        //Please don't use underscore for privates as I don't use underscore in this project^)
        private MainSettings _gameSettings;
        private NightEventProvider _nightEvents;
        public GameData GameData;
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
        public void Construct(EncounterDeckBase deck,
                               MainSettings settings,
                               VisitorManager visitorManager,
                               Cauldron cauldron,
                               NightEventProvider nightEvents)
        {
            _gameSettings = settings;
            _nightEvents = nightEvents;
            //Create it in Zenject^)
            GameData = new GameData(settings.statusBars, deck,nightEvents);

            //We can use Zenject to inject all necessary dependencies into states. 
            //One way to do it is to use a state factory.
            //We bind it in Inject as Container.Bind<StateFactory>().AsSingle(),
            //it calls the constructor and injects everything in it automatically,
            //then we use it as visitorWaitingState = factory.VisitorWaitingState()  
            //This class would be super clean^)
            _visitorWaitingState = new VisitorWaitingState(settings, GameData, this);
            _visitorState = new VisitorState(deck, settings, GameData, visitorManager, cauldron, this);
            _nightState = new NightState(GameData, _gameSettings, _nightEvents, _cardDeck, _nightPanel, this);
            _endGameState = new EndGameState(_endingScreen);
            
            _currentGameState = _visitorWaitingState;
            
            
            Debug.Log("Run StateMachine Construct method");
        }

        private void Start()
        {
            Debug.Log("StateMachine Start");
            //Move to constructor, or better yet to GameplayInstaller if the deck is injectable (please bind it by interface)
            _cardDeck.Init(GameData);
            //I removed cat tutorial, it's not used anymore)
            _currentGameState.Enter();
        }

        //No-no-no, please don't make states publicly accessible, it's the machine's responsibility to know about them.
        //Make them private, collect them into a dictionary with Phase enum key and switch them by the key.
        //That way we can also save the game state as a key, and check not to switch to the same state twice, and clear the current state by calling exit or whatever  
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
        
        //This is used only in visitor waiting state, let's move this delay there
        //The state class can't use a coroutine but the delay can be made with await or through a stopwatch
        //Imagine we decide that the witch can practice between visitors and invite a visitor (i.e. end visitor waiting state) with a user input.
        //We can easily substitute the delay in that state.
        private IEnumerator WaitBeforeSwitch()
        {
            yield return new WaitForSeconds(_gameSettings.gameplay.villagerDelay);
            SwitchState(_currentGameState,false);
        }
    }
}