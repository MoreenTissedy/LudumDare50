using Zenject;

namespace CauldronCodebase.GameStates
{
    //it's a classic OOP factory pattern, unfortunately there is no sense in using Zenject factories here
    public class StateFactory
    {
        EncounterDeckBase deck;
        MainSettings settings;
        VisitorManager visitorManager;
        Cauldron cauldron;
        NightEventProvider nightEvents;
        NightPanel nightPanel;
        EndingScreen endingScreen;
        GameData gameData;
        GameStateMachine gameStateMachine;
        private readonly SoundManager soundManager;

        [Inject]
        public StateFactory(EncounterDeckBase deck,
                            MainSettings settings,
                            VisitorManager visitorManager,
                            Cauldron cauldron,
                            NightEventProvider nightEvents,
                            NightPanel nightPanel,
                            EndingScreen endingScreen,
                            GameData gameData,
                            GameStateMachine gameStateMachine,
                            SoundManager soundManager)
        {
            this.deck = deck;
            this.settings = settings;
            this.visitorManager = visitorManager;
            this.cauldron = cauldron;
            this.nightEvents = nightEvents;
            this.nightPanel = nightPanel;
            this.endingScreen = endingScreen;
            this.gameData = gameData;
            this.gameStateMachine = gameStateMachine;
            this.soundManager = soundManager;
        }

        public VisitorState CreateVisitorState()
        {
            return new VisitorState(deck, settings, gameData, visitorManager, cauldron, gameStateMachine, nightEvents, soundManager);
        }

        public VisitorWaitingState CreateVisitorWaitingState()
        {
            return new VisitorWaitingState(settings, gameData, gameStateMachine);
        }

        public NightState CreateNightState()
        {
            return new NightState(gameData, settings, nightEvents, deck, nightPanel, gameStateMachine);
        }

        public EndGameState CreateEndGameState()
        {
            return new EndGameState(endingScreen, gameStateMachine);
        }

    }
}

