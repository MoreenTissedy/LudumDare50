using CauldronCodebase.GameStates;
using Save;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class GameplayInstaller : MonoInstaller
    {
        [Header("Data Providers")]
        [SerializeField] private RecipeProvider recipeProvider;
        [SerializeField] private NightEventProvider nightEvents;
        [SerializeField] private EncounterDeck encounterDeck;
        [SerializeField] private IngredientsData ingredientsData;
        [SerializeField] private EndingsProvider endings;
        [SerializeField] private PriorityLaneProvider priorityLane;

        [Header("Gameplay")]
        [SerializeField] private RecipeBook recipeBook;
        [SerializeField] private Cauldron theCauldron;
        [SerializeField] private VisitorManager visitorManager;
        [SerializeField] private GameStateMachine stateMachine;
        [SerializeField] private GameDataHandler gameDataHandler;

        [Header("UI")]
        [SerializeField] private EndingScreen endingScreen;

        [SerializeField] private GameFXManager fxManager;

        [SerializeField] private NightPanel nightPanel;

        [Inject] private MainSettings mainSettings;
        [Inject] private DataPersistenceManager dataPersistenceManager;
        [Inject] private SODictionary soDictionary;

        public override void InstallBindings()
        {
            BindDataProviders();
            BindGameplay();
            BindUI();
            Initialize();
        }

        private void BindDataProviders()
        {
            Container.Bind<IngredientsData>().FromInstance(ingredientsData).AsSingle();
            Container.Bind<EncounterDeck>().FromInstance(encounterDeck).AsSingle().NonLazy();
            Container.Bind<RecipeProvider>().FromInstance(recipeProvider).AsSingle();
            Container.Bind<NightEventProvider>().FromInstance(nightEvents).AsSingle();
            Container.Bind<EndingsProvider>().FromInstance(endings).AsSingle();
            Container.Bind<PriorityLaneProvider>().FromInstance(priorityLane).AsSingle();
        }

        private void BindUI()
        {
            Container.Bind<EndingScreen>().FromInstance(endingScreen).AsSingle();
            Container.Bind<NightPanel>().FromInstance(nightPanel).AsSingle();
            Container.Bind<GameFXManager>().FromInstance(fxManager).AsSingle();
        }

        private void BindGameplay()
        {
            Container.Bind<StatusChecker>().FromNew().AsSingle();
            Container.Bind<GameStateMachine>().FromInstance(stateMachine).AsSingle().NonLazy();
            Container.Bind<StateFactory>().AsTransient();
            Container.Bind<RecipeBook>().FromInstance(recipeBook).AsSingle();
            Container.Bind<Cauldron>().FromInstance(theCauldron).AsSingle();
            Container.Bind<VisitorManager>().FromInstance(visitorManager).AsSingle();
            Container.Bind<TooltipManager>().AsSingle().NonLazy();
            Container.Bind<GameDataHandler>().FromInstance(gameDataHandler).AsSingle().NonLazy();
        }

        private void Initialize()
        {
            gameDataHandler.Init(mainSettings, encounterDeck, nightEvents, dataPersistenceManager, soDictionary);
            encounterDeck.Init(gameDataHandler, dataPersistenceManager, soDictionary, mainSettings);
            nightEvents.Init(dataPersistenceManager, soDictionary);
            priorityLane.Init(encounterDeck, soDictionary, dataPersistenceManager);
            endings.Init();
        }
    }
}