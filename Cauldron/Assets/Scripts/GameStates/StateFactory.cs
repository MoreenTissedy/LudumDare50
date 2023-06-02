using Save;
using Zenject;

namespace CauldronCodebase.GameStates
{
    //it's a classic OOP factory pattern, unfortunately there is no sense in using Zenject factories here
    public class StateFactory
    {
        EncounterDeck deck;
        MainSettings settings;
        VisitorManager visitorManager;
        Cauldron cauldron;
        NightEventProvider nightEvents;
        NightPanel nightPanel;
        EndingScreen endingScreen;
        GameDataHandler gameDataHandler;
        GameStateMachine gameStateMachine;
        RecipeBook recipeBook;
        private PriorityLaneProvider priorityLaneProvider;

        DataPersistenceManager dataPersistenceManager;
        private GameFXManager gameFXManager;

        private readonly SoundManager soundManager;

        [Inject]
        public StateFactory(EncounterDeck deck,
                            MainSettings settings,
                            VisitorManager visitorManager,
                            Cauldron cauldron,
                            NightEventProvider nightEvents,
                            NightPanel nightPanel,
                            EndingScreen endingScreen,
                            RecipeBook recipeBook,

                            GameDataHandler gameDataHandler,
                            DataPersistenceManager dataPersistenceManager,
                            PriorityLaneProvider priorityLaneProvider,
                            
                            GameStateMachine gameStateMachine,
                            SoundManager soundManager,
                            GameFXManager fxManager)

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
            this.recipeBook = recipeBook;

            this.dataPersistenceManager = dataPersistenceManager;
            this.priorityLaneProvider = priorityLaneProvider;

            this.soundManager = soundManager;
            gameFXManager = fxManager;

        }

        public VisitorState CreateVisitorState()
        {
            return new VisitorState(deck, settings, gameDataHandler, visitorManager, cauldron,
                gameStateMachine, nightEvents, soundManager, priorityLaneProvider);
        }


        public VisitorWaitingState CreateVisitorWaitingState()
        {
            return new VisitorWaitingState(settings, gameDataHandler, gameStateMachine, gameFXManager);
        }

        public NightState CreateNightState()
        {
            return new NightState(gameDataHandler, settings, nightEvents, deck, nightPanel, 
                gameStateMachine, recipeBook, gameFXManager, priorityLaneProvider);
        }

        public EndGameState CreateEndGameState()
        {
            return new EndGameState(endingScreen, gameStateMachine, dataPersistenceManager, gameFXManager);
        }

    }
}

