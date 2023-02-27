using Save;
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
        GameDataHandler gameDataHandler;
        GameStateMachine gameStateMachine;

        DataPersistenceManager dataPersistenceManager;

        private readonly SoundManager soundManager;

        [Inject]
        public StateFactory(EncounterDeckBase deck,
                            MainSettings settings,
                            VisitorManager visitorManager,
                            Cauldron cauldron,
                            NightEventProvider nightEvents,
                            NightPanel nightPanel,
                            EndingScreen endingScreen,

                            GameDataHandler gameDataHandler,
                            DataPersistenceManager dataPersistenceManager,
                            
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
            this.gameDataHandler = gameDataHandler;
            this.gameStateMachine = gameStateMachine;

            this.dataPersistenceManager = dataPersistenceManager;

            this.soundManager = soundManager;

        }

        public VisitorState CreateVisitorState()
        {
            return new VisitorState(deck, settings, dataPersistenceManager, gameDataHandler, visitorManager, cauldron,
                gameStateMachine, nightEvents, soundManager);
        }


        public VisitorWaitingState CreateVisitorWaitingState()
        {
            return new VisitorWaitingState(settings, gameDataHandler, gameStateMachine);
        }

        public NightState CreateNightState()
        {
            return new NightState(gameDataHandler, settings, nightEvents, deck, nightPanel, gameStateMachine);
        }

        public EndGameState CreateEndGameState()
        {
            return new EndGameState(endingScreen, gameStateMachine, dataPersistenceManager);
        }

    }
}

