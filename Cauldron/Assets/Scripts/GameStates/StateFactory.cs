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
        GameDataHandler gameDataHandler;
        GameStateMachine gameStateMachine;
        RecipeBook recipeBook;

        DataPersistenceManager dataPersistenceManager;
        private GameFXManager gameFXManager;
        private readonly StatusChecker statusChecker;
        private readonly IAchievementManager achievementManager;

        private readonly SoundManager soundManager;

        [Inject]
        public StateFactory(EncounterDeck deck,
                            MainSettings settings,
                            VisitorManager visitorManager,
                            Cauldron cauldron,
                            NightEventProvider nightEvents,
                            NightPanel nightPanel,
                            RecipeBook recipeBook,

                            GameDataHandler gameDataHandler,
                            DataPersistenceManager dataPersistenceManager,
                            GameStateMachine gameStateMachine,
                            SoundManager soundManager,
                            GameFXManager fxManager, 
                            StatusChecker statusChecker, 
                            IAchievementManager achievementManager)

        {
            this.deck = deck;
            this.settings = settings;
            this.visitorManager = visitorManager;
            this.cauldron = cauldron;
            this.nightEvents = nightEvents;
            this.nightPanel = nightPanel;
            this.gameDataHandler = gameDataHandler;
            this.gameStateMachine = gameStateMachine;
            this.recipeBook = recipeBook;

            this.dataPersistenceManager = dataPersistenceManager;

            this.soundManager = soundManager;
            gameFXManager = fxManager;
            this.statusChecker = statusChecker;
            this.achievementManager = achievementManager;
        }

        public VisitorState CreateVisitorState()
        {
            return new VisitorState(deck, settings, gameDataHandler, visitorManager, cauldron,
                gameStateMachine, nightEvents, soundManager, statusChecker);
        }


        public VisitorWaitingState CreateVisitorWaitingState()
        {
            return new VisitorWaitingState(settings, gameDataHandler, gameStateMachine);
        }

        public NightState CreateNightState()
        {
            return new NightState(gameDataHandler, settings, nightEvents, deck, nightPanel, 
                gameStateMachine, recipeBook, gameFXManager, statusChecker, achievementManager);
        }

        public EndGameState CreateEndGameState()
        {
            return new EndGameState(dataPersistenceManager, gameFXManager);
        }

    }
}

